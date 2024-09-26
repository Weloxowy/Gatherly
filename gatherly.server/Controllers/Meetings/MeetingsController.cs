using System.Collections;
using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Mailing.MailEntity;
using gatherly.server.Models.Meetings.Invitations;
using gatherly.server.Models.Meetings.Meeting;
using gatherly.server.Models.Meetings.UserMeeting;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FluentNHibernate.Conventions;
using gatherly.server.Models.Chat.Chat;
using Microsoft.IdentityModel.Tokens;

namespace gatherly.server.Controllers.Meetings;

/// <summary>
///     Controller responsible for handling operations related to meetings.
/// </summary>
/// <remarks>
///     This controller provides endpoints for managing meetings, including creating, deleting, updating, and retrieving meeting information. 
///     It also handles invitations and user-related operations associated with meetings.
/// </remarks>
[ApiController]
[Route("/api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    private readonly IUserMeetingService _userMeetingService;
    private readonly IInvitationsService _invitationsService;
    private readonly IUserEntityService _userService;
    private readonly ITokenEntityService _tokenService;
    private readonly IMailEntityService _mailService;
    private readonly IChatService _chatService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeetingsController"/> class.
    /// </summary>
    /// <param name="mailService">Service for mailing operations.</param>
    /// <param name="meetingService">Service for meeting entity type operations.</param>
    /// <param name="invitationsService">Service for invitations operations.</param>
    /// <param name="userService">Service for user-related operations.</param>
    /// <param name="userMeetingService">Service for joint table userEntity & meetingEntity operations.</param>
    /// <param name="tokenService">Service for token-related operations.</param>
    public MeetingsController(IMeetingService meetingService, IUserMeetingService userMeetingService,
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
    
    /// <summary>
    ///     Retrieves detailed information about a specified meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint retrieves detailed information about a specific meeting. The action can only be performed by a user who is invited to the meeting or is its owner.
    /// </remarks>
    /// <param name="meetingId">The ID of the meeting to be retrieved.</param>
    /// <returns>The meeting entity containing all relevant details.</returns>
    /// <response code="200">Returns the meeting entity.</response>
    /// <response code="401">The requesting user is not authorized to view this meeting.</response>
    /// <response code="404">The specified meeting does not exist.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("{meetingId}")]
    [Authorize]
    public async Task<ActionResult<FullMeetingDTOInfo>> GetMeetingById(Guid meetingId)
    {
        try
        {
            var requestingUser = _tokenService.GetIdFromRequestCookie(HttpContext);
            var meeting = await _meetingService.GetMeetingById(meetingId);
            var user = await _userService.GetUserInfo(meeting.OwnerId);
            if (meeting == null)
            {
                return NotFound("Meeting not found.");
            }
            
            if (user == null)
            {
                return NotFound("Meeting not found.");
            }
            var isUserInMeeting = await _userMeetingService.IsUserInMeeting(Guid.Parse(requestingUser), meetingId);
            if (isUserInMeeting == false)
            {
                return Unauthorized("You are not in the meeting.");
            }
            var fullMeeting = new FullMeetingDTOInfo()
            {
                CreationTime = meeting.CreationTime,
                Description = meeting.Description,
                EndOfTheMeeting = meeting.EndOfTheMeeting,
                StartOfTheMeeting = meeting.StartOfTheMeeting,
                Id = meeting.Id,
                IsMeetingTimePlanned = meeting.IsMeetingTimePlanned,
                Lat = meeting.Lat ?? null,
                Lon = meeting.Lon ?? null,
                MeetingName = meeting.MeetingName,
                OwnerId = meeting.OwnerId,
                PlaceName = meeting.PlaceName,
                TimeZone = meeting.TimeZone,
                OwnerName = user.Name,
                isRequestingUserAnOwner = requestingUser == meeting.OwnerId.ToString()
            };
            return Ok(fullMeeting);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Creates a new meeting and assigns the creator as a confirmed participant.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows an authorized user to create a new meeting. The user creating the meeting is automatically added as a confirmed participant.
    /// </remarks>
    /// <param name="meeting">The details of the meeting to be created.</param>
    /// <returns>The created meeting entity.</returns>
    /// <response code="200">Returns the created meeting entity.</response>
    /// <response code="401">The requesting user is not authorized to create a meeting.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateMeeting([FromBody] MeetingDTOCreate meeting)
    {
        try
        {
         var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
                 if (userId == null)
                 {
                     return NotFound();
                 }

                 if (meeting.EndOfTheMeeting < meeting.StartOfTheMeeting)
                 {
                     return Forbid("End of the meeting is earlier than start of the meeting");
                 }
                 var mt = await _meetingService.CreateNewMeeting(Guid.Parse(userId), meeting);
            var add = await _userMeetingService.CreateNewUserMeetingEntity(new UserMeetingDTOCreate { MeetingId = mt.Id, UserId = Guid.Parse(userId), Status = InvitationStatus.Accepted, Availability = null });
                 return Ok(mt.Id);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Updates the basic information of an existing meeting, such as the name, description, and time zone.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows an authorized user to update the basic details of an existing meeting, including its name, description, and time zone.
    /// </remarks>
    /// <param name="meeting">The updated meeting information.</param>
    /// <returns>A confirmation message indicating the result of the update.</returns>
    /// <response code="200">Successfully updated the meeting.</response>
    /// <response code="404">The specified meeting was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
 [Authorize]
[HttpPatch("{meetingId}")]
public async Task<ActionResult> ChangeMeetingData([FromBody] MeetingDTOUpdate meeting, Guid meetingId)
{
    try
    {
        // Pobranie istniejącego spotkania
        var existingMeeting = await _meetingService.GetMeetingById(meetingId);
        if (existingMeeting == null)
        {
            return NotFound("Meeting not found");
        }

        // Sprawdzenie, czy użytkownik jest właścicielem spotkania
        var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
        var isOwner = await _meetingService.IsUserAnMeetingOwner(meetingId, Guid.Parse(userId));

        if (!isOwner)
        {
            return Unauthorized("User is not the meeting owner");
        }
        
        if (meeting.StartOfTheMeeting.HasValue || meeting.EndOfTheMeeting.HasValue){
        try
        {
            var originalStart = existingMeeting.StartOfTheMeeting;
            var originalEnd = existingMeeting.EndOfTheMeeting;

            var newStart = meeting.StartOfTheMeeting ?? existingMeeting.StartOfTheMeeting;
            var newEnd = meeting.EndOfTheMeeting ?? existingMeeting.EndOfTheMeeting;

            var startOffset = (int)((newStart - originalStart).TotalMinutes / 60);
            var endOffset = (int)((newEnd - originalEnd).TotalMinutes / 60);
            
            var userMeetings = await _userMeetingService.GetAllInvites(meetingId);
            foreach (var user in userMeetings)
            {
                if(startOffset != 0)
                    await _userMeetingService.ChangeAvailbilityTimeFrames(user.Id, startOffset);
                if(endOffset != 0)
                    await _userMeetingService.ChangeAvailbilityTimeFrames(user.Id, endOffset);
            }
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
        }
        
// Aktualizacja strefy czasowej
        TimeZoneInfo newTimezone = meeting.TimeZone != null 
            ? TimeZoneInfo.FindSystemTimeZoneById(meeting.TimeZone) 
            : existingMeeting.TimeZone;
        
// Aktualizacja daty rozpoczęcia spotkania
        if (meeting.StartOfTheMeeting.HasValue)
        {
            // Ustawiamy Kind na Unspecified, bo nie chcemy zakładać, że wejściowy czas jest już w UTC
            DateTime startDateTimeUnspecified = DateTime.SpecifyKind(meeting.StartOfTheMeeting.Value, DateTimeKind.Unspecified);

            // Konwersja daty z nowej strefy czasowej na UTC
            DateTime utcStartDateTime = TimeZoneInfo.ConvertTimeToUtc(startDateTimeUnspecified, newTimezone);

            // Zapisujemy datę w UTC
            existingMeeting.StartOfTheMeeting = utcStartDateTime;
        }
        
// Aktualizacja daty zakończenia spotkania
        if (meeting.EndOfTheMeeting.HasValue)
        {
            // Ustawiamy Kind na Unspecified, bo nie chcemy zakładać, że wejściowy czas jest już w UTC
            DateTime endDateTimeUnspecified = DateTime.SpecifyKind(meeting.EndOfTheMeeting.Value, DateTimeKind.Unspecified);

            // Konwersja daty z nowej strefy czasowej na UTC
            DateTime utcEndDateTime = TimeZoneInfo.ConvertTimeToUtc(endDateTimeUnspecified, newTimezone);

            // Zapisujemy datę w UTC
            existingMeeting.EndOfTheMeeting = utcEndDateTime;
        }
        
        //existingMeeting.StartOfTheMeeting = meeting.StartOfTheMeeting ?? existingMeeting.StartOfTheMeeting;
        //existingMeeting.EndOfTheMeeting = meeting.EndOfTheMeeting ?? existingMeeting.EndOfTheMeeting;
        existingMeeting.MeetingName = meeting.MeetingName ?? existingMeeting.MeetingName;
        existingMeeting.Description = meeting.Description ?? existingMeeting.Description;
        existingMeeting.PlaceName = meeting.PlaceName ?? existingMeeting.PlaceName;
        existingMeeting.Lon = meeting.Lon ?? existingMeeting.Lon;
        existingMeeting.Lat = meeting.Lat ?? existingMeeting.Lat;
        existingMeeting.IsMeetingTimePlanned = meeting.IsMeetingTimePlanned ?? existingMeeting.IsMeetingTimePlanned;
        existingMeeting.TimeZone = newTimezone;

        // Zapisz zmiany
        await _meetingService.UpdateAllMeetingData(meetingId, existingMeeting);
        await _chatService.SaveSystemMessageAsync(meetingId,
            $"Dokonano zmiany szczegółów spotkania.");
        return Ok("Updated correctly");
    }
    catch (Exception ex)
    {
        // Możesz również dodać logowanie błędu tutaj, jeśli potrzebujesz więcej informacji do debugowania
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
    
    /// <summary>
    ///     Updates the start and end times of an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows an authorized user to update the start and end times of an existing meeting. It also adjusts the availability time frames for all invited users accordingly.
    /// </remarks>
    /// <param name="meeting">The updated meeting time details.</param>
    /// <returns>A confirmation message indicating the result of the update.</returns>
    /// <response code="200">Successfully updated the meeting times.</response>
    /// <response code="404">The specified meeting was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpPost("updateTime")]
    public async Task<ActionResult> ChangeTimeFrame([FromBody] MeetingDTOUpdateDate meeting)
    {
        try
        {
            var existingMeeting = await _meetingService.GetMeetingById(meeting.Id);
            if (existingMeeting == null)
            {
                return NotFound("Meeting not found");
            }

            var originalStart = existingMeeting.StartOfTheMeeting;
            var originalEnd = existingMeeting.EndOfTheMeeting;

            var newStart = meeting.StartOfTheMeeting;
            var newEnd = meeting.EndOfTheMeeting;

            var startOffset = (int)((newStart - originalStart).TotalMinutes / 60);
            var endOffset = (int)((newEnd - originalEnd).TotalMinutes / 60);

            existingMeeting.StartOfTheMeeting = newStart;
            existingMeeting.EndOfTheMeeting = newEnd;

            await _meetingService.UpdateAllMeetingData(meeting.Id, existingMeeting);
        
            var userMeetings = await _userMeetingService.GetAllInvites(meeting.Id);
            foreach (var user in userMeetings)
            {
                if(startOffset != 0)
                    await _userMeetingService.ChangeAvailbilityTimeFrames(user.Id, startOffset);
                if(endOffset != 0)
                    await _userMeetingService.ChangeAvailbilityTimeFrames(user.Id, endOffset);
            }
            await _chatService.SaveSystemMessageAsync(meeting.Id,
                $"Zmieniono czas spotkania.");
            return Ok("Updated correctly");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    ///     Changes the planning mode of an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows an authorized user to change the planning mode of an existing meeting, such as from a specified time mode to a planning mode.
    /// </remarks>
    /// <param name="id">The ID of the meeting whose planning mode is to be changed.</param>
    /// <returns>A confirmation message indicating the result of the action.</returns>
    /// <response code="200">Successfully changed the planning mode.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpPost("changeMode/{meetingId}")]
    public async Task<IActionResult> ChangeMeetingPlaningMode(Guid meetingId)
    {
        try
        { 
            await _chatService.SaveSystemMessageAsync(meetingId,
                "Tryb planowania spotkania został zmieniony");
            await _meetingService.ChangeMeetingPlaningMode(meetingId);
            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Retrieves all available time zones.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all time zones available on the system.
    /// </remarks>
    /// <returns>A list of time zone identifiers.</returns>
    /// <response code="200">Successfully retrieved the list of time zones.</response>
    [AllowAnonymous]
    [HttpGet("getAllTimeZones")]
    public async Task<IActionResult> GetAllTimeZones()
    {
        Dictionary<string, string> ninfo = new Dictionary<string, string>();
        List<string> info = new List<string>();
        foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
        {
            ninfo.Add(z.Id,z.DisplayName);
            info.Add(z.Id);
        }

        return Ok(ninfo);
    }
    
    /// <summary>
    ///     Retrieves information about the user's next upcoming meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns details about the next upcoming meeting for a specified user, ordered by the start time.
    /// </remarks>
    /// <returns>The details of the next meeting.</returns>
    /// <response code="200">Returns the details of the next meeting.</response>
    /// <response code="401">The requesting user is not authorized to view this information.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("nextMeeting")]
    [Authorize]
    public async Task<ActionResult> GetNextMeetingInfo()
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (userId.IsEmpty())
            {
                return NotFound("User not found.");
            }
            
            var meetings = await _userMeetingService.GetAllMeetingsByUserId(Guid.Parse(userId));
            if (meetings.Count > 0)
            {
                var meeting = meetings.Where(x => x.StartOfTheMeeting > DateTime.UtcNow).MinBy(x => x.StartOfTheMeeting);
                /*if (meeting != null)
                {
                    meeting.StartOfTheMeeting = TimeZoneInfo.ConvertTimeToUtc(meeting.StartOfTheMeeting, meeting.TimeZone);  
                }*/
                return Ok(meeting);
            }

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    ///     Retrieves information about the user's next 5 upcoming meetings.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns details about the next 5 upcoming meetings for a specified user, ordered by the start time.
    /// </remarks>
    /// <returns>A list of the next 5 meetings.</returns>
    /// <response code="200">Returns the details of the next 5 meetings.</response>
    /// <response code="401">The requesting user is not authorized to view this information.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("nextMeetings")]
    [Authorize]
    public async Task<ActionResult> GetNextMeetingsInfo()
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (userId.IsEmpty())
            {
                return NotFound("User not found.");
            }
            
            var meetings = await _userMeetingService.GetAllMeetingsByUserId(Guid.Parse(userId));
            return Ok(meetings.Where(x=>x.StartOfTheMeeting > DateTime.UtcNow).OrderBy(x => x.StartOfTheMeeting).Take(5));
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    ///     Deletes a meeting and associated user-meeting entities.
    /// </summary>
    /// <remarks>
    ///     This endpoint deletes a meeting and all associated user-meeting entities. Only the meeting owner can perform this action.
    /// </remarks>
    /// <param name="meetingId">The ID of the meeting to be deleted.</param>
    /// <returns>A confirmation message indicating the result of the deletion.</returns>
    /// <response code="200">Successfully deleted the meeting.</response>
    /// <response code="401">The requesting user is not authorized to delete this meeting.</response>
    /// <response code="404">The specified meeting was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpDelete("{meetingId}")]
    [Authorize]
    public async Task<ActionResult> DeleteMeeting(string meetingId)
    {
        var meeting = await _meetingService.GetMeetingById(Guid.Parse(meetingId));
        if (meeting == null)
        {
            return NotFound("Meeting not found");
        }
            
        var owner = _tokenService.GetIdFromRequestCookie(HttpContext);
        if (owner.IsNullOrEmpty() || !owner.Equals(meeting.OwnerId.ToString()))
        {
            return Unauthorized();
        }
        try
        {
            var delete = await _meetingService.DeleteMeeting(Guid.Parse(meetingId));
            if (delete == false)
            {
                return NotFound();
            }
            var users = await _userMeetingService.GetAllInvites(Guid.Parse(meetingId));
            foreach (var user in users)
            {
                var userMeetingId = await _userMeetingService.GetUserMeetingId(user.UserId, Guid.Parse(meetingId));
                if (userMeetingId != null)
                {
                    await _userMeetingService.DeleteUserMeetingEntity(user.UserId, Guid.Parse(meetingId));
                    await _mailService.SendMeetingDeletedAsync(user.Name,user.Email,meeting.MeetingName); 
                }
            }
            return Ok("Deleted successfully");

        }
        catch(Exception ex)
        {
            return StatusCode(500, "Internal server error"+ex);
        }
    }

    /// <summary>
    ///     Prepares possible dates and times for a meeting based on user availability.
    /// </summary>
    /// <remarks>
    ///     This endpoint generates potential dates and times for a meeting by analyzing the availability of invited users.
    /// </remarks>
    /// <param name="meetingId">The ID of the meeting for which possible dates and times are being prepared.</param>
    /// <returns>A serialized list of possible date and time slots for the meeting.</returns>
    /// <response code="200">Successfully generated possible dates and times.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpPost("prepareTimes")]
    [Authorize]
    public async Task<ActionResult> PreparePosibleDateTimesForUsers(Guid meetingId)
    {
        try
        {
            var res = await _userMeetingService.PreparePosibleDateTimes(meetingId);
            return Ok(JsonSerializer.Serialize(res));
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    ///     Retrieves the number of meetings scheduled for today for the authenticated user.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns the number of meetings that the authenticated user has scheduled for the current day.
    /// </remarks>
    /// <returns>The number of meetings scheduled for today.</returns>
    /// <response code="200">Returns the number of meetings for today.</response>
    /// <response code="404">The authenticated user was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("numberOfMeetings")]
    [Authorize]
    public async Task<ActionResult> GetNumberOfMeetings()
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if(userId == null)
            {
                return NotFound("User not found");
            }
            var numOfMeetings = await _userMeetingService.CountAllMeetingsByUserId(Guid.Parse(userId));
     
            return Ok(numOfMeetings);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    ///     Retrieves all participants invited to a specified meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all users invited to a specific meeting, regardless of their response status.
    /// </remarks>
    /// <param name="meetingId">The ID of the meeting for which participants are to be retrieved.</param>
    /// <returns>A list of all invited participants.</returns>
    /// <response code="200">Returns a list of all participants invited to the meeting.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("all/{meetingId}")]
    [Authorize]
    public async Task<ActionResult> GetAllParticipants(string meetingId)
    {
        try
        {
            var invites = await _userMeetingService.GetAllInvites(Guid.Parse(meetingId));
            return Ok(JsonSerializer.Serialize(invites));
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Retrieves all participants who have confirmed their attendance for a specified meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all users who have confirmed their attendance for a specific meeting.
    /// </remarks>
    /// <param name="meetingId">The ID of the meeting for which confirmed participants are to be retrieved.</param>
    /// <returns>A list of confirmed participants.</returns>
    /// <response code="200">Returns a list of all participants who have confirmed their attendance.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("confirmed/{meetingId}")]
    [Authorize]
    public async Task<ActionResult> GetAllConfirmedParticipants(string meetingId)
    {
        try
        {
            var invites = await _userMeetingService.GetAllConfirmedInvites(Guid.Parse(meetingId));
            return Ok(JsonSerializer.Serialize(invites));
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    ///     Retrieves all participants who have neither confirmed nor rejected their attendance for a specified meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all users who have not yet responded to an invitation to a specific meeting.
    /// </remarks>
    /// <param name="meetingId">The ID of the meeting for which pending participants are to be retrieved.</param>
    /// <returns>A list of pending participants.</returns>
    /// <response code="200">Returns a list of all participants with pending attendance status.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("pending/{meetingId}")]
    [Authorize]
    public async Task<ActionResult> GetAllPendingParticipants(string meetingId)
    {
        try
        {
            var invites = await _userMeetingService.GetAllPendingInvites(Guid.Parse(meetingId));
            return Ok(JsonSerializer.Serialize(invites));
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    ///     Retrieves all participants who have rejected their attendance for a specified meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns a list of all users who have declined the invitation to a specific meeting.
    /// </remarks>
    /// <param name="meetingId">The ID of the meeting for which rejected participants are to be retrieved.</param>
    /// <returns>A list of rejected participants.</returns>
    /// <response code="200">Returns a list of all participants who have rejected their attendance.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("rejected/{meetingId}")]
    [Authorize]
    public async Task<ActionResult> GetAllRejectedParticipants(string meetingId)
    {
        try
        {
            var invites = await _userMeetingService.GetAllRejectedInvites(Guid.Parse(meetingId));
            return Ok(JsonSerializer.Serialize(invites));
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Retrieves information about the user's meetings.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns details about all meetings for a specified user.
    /// </remarks>
    /// <returns>A list of the next 5 meetings.</returns>
    /// <response code="200">Returns the details of the meetings.</response>
    /// <response code="401">The requesting user is not authorized to view this information.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("allMeetings")]
    [Authorize]
    public async Task<ActionResult> GetAllMeetings()
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (userId.IsEmpty())
            {
                return NotFound("User not found.");
            }
            
            var meetings = await _userMeetingService.GetAllMeetingsByUserId(Guid.Parse(userId));
            return Ok(meetings.ToList());
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    /// <summary>
    ///     Removes user from an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows an meeting owner to delete user from it.
    /// </remarks>
    /// <param name="userMeeting">Id's of meeting and user who will be removed.</param>
    /// <returns>A confirmation message indicating the result of the update.</returns>
    /// <response code="200">Successfully removed user.</response>
    /// <response code="404">The specified user-meeting pair was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpDelete("meeting/deleteUser")]
    public async Task<ActionResult> DeleteUserFromMeeitng([FromBody] InvitationDTO userMeeting)
    {
        try
        {
            var existingMeeting = await _meetingService.GetMeetingById(userMeeting.MeetingId);
            if (existingMeeting == null)
            {
                return NotFound("Meeting not found");
            }
            var newUserMeeting = await _userMeetingService.GetInviteByIds(userMeeting.MeetingId, userMeeting.UserId);
            if (newUserMeeting == null)
            {
                return NotFound("Meeting not found");
            }
            if (userMeeting.UserId == existingMeeting.OwnerId)
            {
                return Unauthorized("Owner cannot delete themselves out of the meeting");
            }
            var user = await _userService.GetUserInfo(userMeeting.UserId);
            await _userMeetingService.DeleteUserMeetingEntity(newUserMeeting.Id);
            await _chatService.SaveSystemMessageAsync(userMeeting.MeetingId,
                $"Użytkownik {user.Name} został usunięty ze spotkania.");

            return Ok("Deleted correctly");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Removes user from an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows an user to leave meeting.
    /// </remarks>
    /// <param name="meetingId">Id of the meeting that the user will leave.</param>
    /// <returns>A confirmation message indicating the result of the update.</returns>
    /// <response code="200">Successfully removed user.</response>
    /// <response code="404">The specified user-meeting pair was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpDelete("meeting/leave")]
    public async Task<ActionResult> LeaveMeeitng(Guid meetingId)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if(userId == null)
            {
                return NotFound("User not found");
            }
            
            var existingMeeting = await _meetingService.GetMeetingById(meetingId);
            if (existingMeeting == null)
            {
                return NotFound("Meeting not found");
            }
            if (existingMeeting.OwnerId == Guid.Parse(userId))
            {
                return NotFound("Owner cannot leave the meeting");
            }
            var newUserMeeting = await _userMeetingService.GetInviteByIds(meetingId, Guid.Parse(userId));
            if (newUserMeeting == null)
            {
                return NotFound("Meeting not found");
            }
            await _userMeetingService.DeleteUserMeetingEntity(newUserMeeting.Id);
            var user = await _userService.GetUserInfo(Guid.Parse(userId));
            await _chatService.SaveSystemMessageAsync(meetingId,
                $"Użytkownik {user.Name} wyszedł ze spotkania.");

            return Ok("Deleted correctly");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Changes the invitation status of a user in an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows a user to update their invitation status in a meeting, 
    ///     such as accepting, rejecting, or setting it as tentative.
    /// </remarks>
    /// <param name="meetingId">ID of meeting.</param>
    /// <param name="invitationStatus">The new status of the invitation. It must be a valid value from the InvitationStatus enum.</param>
    /// <returns>A confirmation message indicating the result of the status update.</returns>
    /// <response code="200">Successfully updated the invitation status.</response>
    /// <response code="400">The specified invitation status is invalid.</response>
    /// <response code="404">The specified user-meeting pair was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpPut("meeting/setStatus")]
    public async Task<ActionResult> SetInvitationStatus(Guid meetingId,InvitationStatus invitationStatus)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if(userId == null)
            {
                return NotFound("User not found");
            }
            var userMeetingId = await _userMeetingService.GetUserMeetingId(Guid.Parse(userId), meetingId);
            if (userMeetingId.HasValue == false)
            {
                return NotFound("Meeting not found");
            }
            if (!Enum.IsDefined(typeof(InvitationStatus), invitationStatus))
            {
                return BadRequest("Invitation status value is not correct");
            }
            await _userMeetingService.ChangeInvitationStatus(userMeetingId.Value,invitationStatus);
            var user = await _userService.GetUserInfo(Guid.Parse(userId));
            await _chatService.SaveSystemMessageAsync(meetingId,
                $"Użytkownik {user.Name} zmienił status na \"{invitationStatus}\".");

            return Ok("Status updated correctly");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Changes the invitation status of a user in an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows a user to update their invitation status in a meeting, 
    ///     such as accepting, rejecting, or setting it as tentative.
    /// </remarks>
    /// <param name="meetingId">ID of meeting.</param>
    /// <param name="invitationStatus">The new status of the invitation. It must be a valid value from the InvitationStatus enum.</param>
    /// <returns>A confirmation message indicating the result of the status update.</returns>
    /// <response code="200">Successfully updated the invitation status.</response>
    /// <response code="400">The specified invitation status is invalid.</response>
    /// <response code="404">The specified user-meeting pair was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpGet("meeting/getStatus")]
    public async Task<ActionResult> GetInvitationStatus(Guid meetingId)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if(userId == null)
            {
                return NotFound("User not found");
            }
            var userMeetingStatus = await _userMeetingService.GetUserMeetingStatus(Guid.Parse(userId), meetingId);
            if (userMeetingStatus.HasValue == false)
            {
                return NotFound("Meeting not found");
            }
            return Ok(userMeetingStatus);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    
     /// <summary>
    ///     Changes the invitation status of a user in an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows a user to update their invitation status in a meeting, 
    ///     such as accepting, rejecting, or setting it as tentative.
    /// </remarks>
    /// <param name="meetingId">ID of meeting.</param>
    /// <param name="invitationStatus">The new status of the invitation. It must be a valid value from the InvitationStatus enum.</param>
    /// <returns>A confirmation message indicating the result of the status update.</returns>
    /// <response code="200">Successfully updated the invitation status.</response>
    /// <response code="400">The specified invitation status is invalid.</response>
    /// <response code="404">The specified user-meeting pair was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpPut("meeting/setAvailability")]
    public async Task<ActionResult> SetAvailability(Guid meetingId,string availability)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if(userId == null)
            {
                return NotFound("User not found");
            }
            var userMeetingId = await _userMeetingService.GetUserMeetingId(Guid.Parse(userId), meetingId);
            if (userMeetingId.HasValue == false)
            {
                return NotFound("Meeting not found");
            }

            int byteCount = availability.Length;
            var availabilityBits = availability.ToCharArray();
            byte[] result = new byte[byteCount];
            // Przekształcenie liczby całkowitej na tablicę bajtów
            for (int i = 0; i < byteCount; i++)
            {
                if (availabilityBits[i] == '1')
                {
                    result[i] = (byte)(1);
                }
                else
                {
                    result[i] = (byte)(0);
                }
            }
            
           
            await _userMeetingService.ChangeAvailbilityTimes(userMeetingId.Value,result);
            //var user = _userService.GetUserInfo(Guid.Parse(userId));
            //await _chatService.SaveSystemMessageAsync(meetingId,
            //$"Użytkownik {user.Name} zmienił status na \"{invitationStatus}\".");

            return Ok("Status updated correctly");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Changes the invitation status of a user in an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows a user to update their invitation status in a meeting, 
    ///     such as accepting, rejecting, or setting it as tentative.
    /// </remarks>
    /// <param name="meetingId">ID of meeting.</param>
    /// <param name="invitationStatus">The new status of the invitation. It must be a valid value from the InvitationStatus enum.</param>
    /// <returns>A confirmation message indicating the result of the status update.</returns>
    /// <response code="200">Successfully updated the invitation status.</response>
    /// <response code="400">The specified invitation status is invalid.</response>
    /// <response code="404">The specified user-meeting pair was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpGet("meeting/getAvailability/{meetingId}")]
    public async Task<ActionResult> GetAvailbility(Guid meetingId)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if(userId == null)
            {
                return NotFound("User not found");
            }
            var userMeetingStatus = await _userMeetingService.GetUserTimes(Guid.Parse(userId), meetingId);
            return Ok(userMeetingStatus);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Changes the invitation status of a user in an existing meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows a user to update their invitation status in a meeting, 
    ///     such as accepting, rejecting, or setting it as tentative.
    /// </remarks>
    /// <param name="meetingId">ID of meeting.</param>
    /// <param name="invitationStatus">The new status of the invitation. It must be a valid value from the InvitationStatus enum.</param>
    /// <returns>A confirmation message indicating the result of the status update.</returns>
    /// <response code="200">Successfully updated the invitation status.</response>
    /// <response code="400">The specified invitation status is invalid.</response>
    /// <response code="404">The specified user-meeting pair was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpGet("meeting/getAllAvailability/{meetingId}")]
    public async Task<ActionResult> GetAllAvailbility(Guid meetingId)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if(userId == null)
            {
                return NotFound("User not found");
            }

            var meeting = await _meetingService.GetMeetingById(meetingId);
            if (meeting == null)
            {
                return NotFound("Meeting not found");
            }

            if (!meeting.OwnerId.Equals(Guid.Parse(userId)))
            {
                return Unauthorized("You are not an owner of the meeting.");
            }
            
            var userMeetingStatus = await _userMeetingService.GetAllUserTimes(meetingId,Guid.Parse(userId));
            return Ok(userMeetingStatus);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
}