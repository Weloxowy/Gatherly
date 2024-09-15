using gatherly.server.Entities.Chat;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Chat.Chat;
using NHibernate;
using NHibernate.Linq;

namespace gatherly.server.Persistence.Chat;

public class ChatRepository : IChatRepository
{
    private readonly ISessionFactory _sessionFactory;

    public ChatRepository(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }
    
    public async Task SaveMessageAsync(Message message)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.SaveAsync(message);
            await transaction.CommitAsync();
        }
    }
    
    public async Task<List<MessagesDTO>> GetLastMessagesAsync(Guid meetingId, Guid userId)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            var messages = await session.Query<Message>()
                .Where(m => m.MeetingId == meetingId)
                .OrderByDescending(m => m.Timestamp)
                .Join(session.Query<Models.Authentication.UserEntity.UserEntity>(),
                    message => message.SenderId,
                    userEntity => userEntity.Id,
                    (message, userEntity) => new { Message = message, UserEntity = userEntity })
                .Take(100)
                .ToListAsync();
            messages.Reverse();
            var list = new List<MessagesDTO>();
            foreach (var message in messages)
            {
                var type = TypesOfMessage.OtherUsers;
                if (message.Message.SenderId == userId)
                {
                    type = TypesOfMessage.Me;
                }
                else if (message.Message.SenderId == Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"))
                {
                    type = TypesOfMessage.System;
                }
                list.Add(new MessagesDTO
                {
                    Id = message.Message.Id,
                    SenderId = message.Message.SenderId,
                    Content = message.Message.Content,
                    MeetingId = message.Message.MeetingId,
                    Timestamp = message.Message.Timestamp,
                    TypesOfMessage = type,
                    UserName = message.UserEntity.Name,
                    UserAvatar = message.UserEntity.AvatarName
                });
            }
            
            return list;
        }
    }

    public async Task<List<Message>> GetMessagesFromDateAsync(Guid meetingId, DateTime startDate, int count)
    {
        using (var session = _sessionFactory.OpenSession())
        {
            return await session.Query<Message>()
                .Where(m => m.MeetingId == meetingId && m.Timestamp >= startDate)
                .OrderBy(m => m.Timestamp)
                .Take(count)
                .ToListAsync();
        }
    }
    
    public async Task SaveSystemMessageAsync(Guid meetingId, string text)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            Message message = new Message()
            {
                Content = text,
                MeetingId = meetingId,
                SenderId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"),
                Timestamp = DateTime.UtcNow
            };
            await session.SaveAsync(message);
            await transaction.CommitAsync();
        }
    }
}