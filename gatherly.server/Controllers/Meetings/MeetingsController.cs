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
        IMailEntityService mailService)
    {
        _meetingService = meetingService;
        _userMeetingService = userMeetingService;
        _invitationsService = invitationsService;
        _userService = userService;
        _tokenService = tokenService;
        _mailService = mailService;
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
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Meeting>> GetMeetingById(Guid meetingId)
    {
        try
        {
            var meeting = await _meetingService.GetMeetingById(meetingId);
            if (meeting == null)
            {
                return NotFound("Meeting not found.");
            }
            return Ok(meeting);
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
                 var mt = await _meetingService.CreateNewMeeting(Guid.Parse(userId), meeting);
            var add = await _userMeetingService.CreateNewUserMeetingEntity(new UserMeetingDTOCreate { MeetingId = mt.Id, UserId = Guid.Parse(userId), Status = InvitationStatus.Accepted, Availability = null });
                 return Ok(mt);
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
    [HttpPost("updateBasic")]
    public async Task<ActionResult> ChangeMeetingBasicData([FromBody] MeetingDTOUpdateBasic meeting)
    {
        try
        {
            var existingMeeting = await _meetingService.GetMeetingById(meeting.Id);
            if (existingMeeting == null)
            {
                return NotFound("Meeting not found");
            }

            existingMeeting.MeetingName = meeting.MeetingName;
            existingMeeting.Description = meeting.Description;
            existingMeeting.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(meeting.TimeZone);

            await _meetingService.UpdateAllMeetingData(meeting.Id, existingMeeting);

            return Ok("Updated correctly");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Updates the location details of an existing meeting, including the place name and geographical coordinates.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows an authorized user to update the location information of an existing meeting, such as the place name and its longitude and latitude.
    /// </remarks>
    /// <param name="meeting">The updated location details of the meeting.</param>
    /// <returns>A confirmation message indicating the result of the update.</returns>
    /// <response code="200">Successfully updated the meeting location.</response>
    /// <response code="404">The specified meeting was not found.</response>
    /// <response code="500">An internal server error occurred.</response>
    [Authorize]
    [HttpPost("updateLocation")]
    public async Task<ActionResult> ChangeMeetingLocationData([FromBody] MeetingDTOUpdateLocation meeting)
    {
        try
        {
            var existingMeeting = await _meetingService.GetMeetingById(meeting.Id);
            if (existingMeeting == null)
            {
                return NotFound("Meeting not found");
            }

            existingMeeting.PlaceName = meeting.PlaceName;
            existingMeeting.Lon = meeting.Lon;
            existingMeeting.Lat = meeting.Lat;

            await _meetingService.UpdateAllMeetingData(meeting.Id, existingMeeting);

            return Ok("Updated correctly");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
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

            var startOffset = (int)((newStart - originalStart).TotalMinutes / 15);
            var endOffset = (int)((newEnd - originalEnd).TotalMinutes / 15);

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
    [HttpPost("changeMode")]
    public async Task<IActionResult> ChangeMeetingPlaningMode(Guid id)
    {
        try
        { 
            await _meetingService.ChangeMeetingPlaningMode(id);
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
        List<string> info = new List<string>();
        foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
        {
            info.Add(z.Id);
        }

        return Ok(info);
    }
    
    /// <summary>
    ///     Retrieves information about the user's next upcoming meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns details about the next upcoming meeting for a specified user, ordered by the start time.
    /// </remarks>
    /// <param name="userId">The ID of the user whose next meeting is to be retrieved.</param>
    /// <returns>The details of the next meeting.</returns>
    /// <response code="200">Returns the details of the next meeting.</response>
    /// <response code="401">The requesting user is not authorized to view this information.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("nextMeeting")]
    [Authorize]
    public async Task<ActionResult> GetNextMeetingInfo(string userId)
    {
        try
        {
            var meetings = await _userMeetingService.GetAllMeetingsByUserId(Guid.Parse(userId));
            return Ok(meetings.OrderBy(x => x.StartOfTheMeeting).Take(1));
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
    /// <param name="userId">The ID of the user whose next 5 meetings are to be retrieved.</param>
    /// <returns>A list of the next 5 meetings.</returns>
    /// <response code="200">Returns the details of the next 5 meetings.</response>
    /// <response code="401">The requesting user is not authorized to view this information.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("nextMeetings")]
    [Authorize]
    public async Task<ActionResult> GetNextMeetingsInfo(string userId)
    {
        try
        {
            var meetings = await _userMeetingService.GetAllMeetingsByUserId(Guid.Parse(userId));
            return Ok(meetings.OrderBy(x => x.StartOfTheMeeting).Take(5));
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
    [HttpPost("delete")]
    [Authorize]
    public async Task<ActionResult> DeleteMeeting(string meetingId)
    {
        try
        {
            var meeting = await _meetingService.GetMeetingById(Guid.Parse(meetingId));
            if (meeting == null)
            {
                return NotFound("Meeting not found");
            }

            var owner = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (!owner.Equals(meeting.OwnerId.ToString()))
            {
                return Unauthorized();
            }
            var delete = await _meetingService.DeleteMeeting(Guid.Parse(meetingId));
            if (delete == false)
            {
                return NotFound();
            }
            var users = await _userMeetingService.GetAllInvites(Guid.Parse(meetingId));
            foreach (var user in users)
            {
               await _mailService.SendMeetingDeletedAsync(user.Name,user.Email,meeting.MeetingName);
            }
            return Ok("Deleted successfully");

        }
        catch
        {
            return StatusCode(500, "Internal server error");
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
}