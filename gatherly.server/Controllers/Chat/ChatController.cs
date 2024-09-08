using gatherly.server.Controllers.Meetings;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Chat.Chat;
using gatherly.server.Models.Mailing.MailEntity;
using gatherly.server.Models.Meetings.Invitations;
using gatherly.server.Models.Meetings.Meeting;
using gatherly.server.Models.Meetings.UserMeeting;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gatherly.server.Controllers.Chat;


[ApiController]
[Route("/api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    private readonly IUserMeetingService _userMeetingService;
    private readonly IInvitationsService _invitationsService;
    private readonly IUserEntityService _userService;
    private readonly ITokenEntityService _tokenService;
    private readonly IMailEntityService _mailService;
    private readonly IChatService _chatService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatController"/> class.
    /// </summary>
    /// <param name="mailService">Service for mailing operations.</param>
    /// <param name="meetingService">Service for meeting entity type operations.</param>
    /// <param name="invitationsService">Service for invitations operations.</param>
    /// <param name="userService">Service for user-related operations.</param>
    /// <param name="userMeetingService">Service for joint table userEntity & meetingEntity operations.</param>
    /// <param name="tokenService">Service for token-related operations.</param>
    public ChatController(IMeetingService meetingService, IUserMeetingService userMeetingService,
        IInvitationsService invitationsService, IUserEntityService userService, ITokenEntityService tokenService,
        IMailEntityService mailService, IChatService chatService)
    {
        _meetingService = meetingService;
        _userMeetingService = userMeetingService;
        _invitationsService = invitationsService;
        _userService = userService;
        _tokenService = tokenService;
        _mailService = mailService;
        _chatService = chatService;
    }
    [HttpGet]
    public async Task<ActionResult> LoadMessageHistory(string meetingId, DateTime? startDate = null)
    {
        var user = _tokenService.GetIdFromRequestCookie(HttpContext);
        var meetings = await _userMeetingService.GetAllMeetingsByUserId(Guid.Parse(user));
        var meetingExists = meetings.Any(m => m.Id == Guid.Parse(meetingId));

        if (!meetingExists)
        {
            throw new UnauthorizedAccessException("User is not allowed to access this meeting's chat history.");
        }

        // Pobranie 20 ostatnich wiadomości lub od konkretnej daty
        var messages = startDate.HasValue 
            ? /*await _chatService.GetMessagesFromDateAsync(Guid.Parse(meetingId), startDate.Value, 20)*/ null //potem podmienic funkcja z datą na DTO
            : await _chatService.GetLastMessagesAsync(Guid.Parse(meetingId),Guid.Parse(user));
        return Ok(messages);

    }
}