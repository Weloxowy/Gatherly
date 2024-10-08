﻿using gatherly.server.Entities.Chat;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Chat.Chat;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.SignalR;

namespace gatherly.server.Persistence.Chat
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ITokenEntityService _tokenEntityService;
        private readonly IUserEntityService _userEntityService;

        public ChatHub(IChatService chatService, ITokenEntityService tokenEntityService, IUserEntityService userEntityService)
        {
            _chatService = chatService;
            _tokenEntityService = tokenEntityService;
            _userEntityService = userEntityService;
        }

        public override async Task OnConnectedAsync()
        {
            var user = _tokenEntityService.GetIdFromRequestCookie(Context.GetHttpContext());
            var meeting = Context.GetHttpContext().Request.Query["meetingId"].ToString();
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(meeting))
            {
                Context.Abort();
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, meeting);
            await LoadMessageHistory(meeting);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var meetingId = Context.GetHttpContext().Request.Query["meetingId"].ToString();
            if (!string.IsNullOrEmpty(meetingId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, meetingId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string content)
        {
            var userId = _tokenEntityService.GetIdFromRequestCookie(Context.GetHttpContext());
            var user = await _userEntityService.GetUserInfo(Guid.Parse(userId));
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var senderId = new Guid(userId);
            var meeting = Context.GetHttpContext().Request.Query["meetingId"].ToString();
            var message = new Message
            {
                SenderId = senderId,
                Content = content,
                Timestamp = DateTime.UtcNow,
                MeetingId = Guid.Parse(meeting),
            };
            await _chatService.SaveMessageAsync(message);
            var outMessage = new MessagesDTO
            {
                SenderId = senderId,
                Content = content,
                Timestamp = DateTime.UtcNow,
                MeetingId = Guid.Parse(meeting),
                TypesOfMessage = TypesOfMessage.OtherUsers,
                UserAvatar = user.AvatarName,
                UserName = user.Name
            };
            await Clients.GroupExcept(meeting, Context.ConnectionId).SendAsync("ReceiveMessage", user, outMessage);
            //await Clients.Group(meeting).SendAsync("ReceiveMessage", user, outMessage);
            outMessage.TypesOfMessage = TypesOfMessage.Me;
            await Clients.Caller.SendAsync("ReceiveMessage",user, outMessage);
            //await Clients.All.SendAsync("ReceiveMessage", user, outMessage);
        }

        public async Task LoadMessageHistory(string meeting)
        {
            var user = _tokenEntityService.GetIdFromRequestCookie(Context.GetHttpContext());

            if (user == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            // Pobranie 20 ostatnich wiadomości
            var messages = await _chatService.GetLastMessagesAsync(Guid.Parse(meeting),Guid.Parse(user));
            
            // Przesłanie wiadomości do klienta
            await Clients.Caller.SendAsync("ReceiveMessageHistory", messages);
        }
    }
}
