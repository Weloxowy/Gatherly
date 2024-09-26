using gatherly.server.Entities.Meetings;
using gatherly.server.Models.Meetings.Invitations;
using gatherly.server.Models.Meetings.UserMeeting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentNHibernate.Conventions;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Chat.Chat;
using gatherly.server.Models.Meetings.Meeting;
using gatherly.server.Models.Tokens.TokenEntity;

namespace gatherly.server.Controllers.Meetings;

/// <summary>
///     Controller responsible for handling operations related to inviting users to meetings.
/// </summary>
/// <remarks>
///     This controller provides endpoints for managing invitations to meetings, including creating, confirming, declining, and retrieving invitations. 
/// </remarks>
[ApiController]
[Route("/api/[controller]")]
public class InvitationsController : ControllerBase
{
    private readonly IUserMeetingService _userMeetingService;
    private readonly IInvitationsService _invitationsService;
    private readonly IMeetingService _meetingService;
    private readonly ITokenEntityService _tokenService;
    private readonly IUserEntityService _userEntityService;
    private readonly IChatService _chatService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvitationsController"/> class.
    /// </summary>
    /// <param name="userMeetingService">Service for managing operations related to the relationship between users and meetings.</param>
    /// <param name="invitationsService">Service for handling invitation-related operations.</param>
    /// <param name="meetingService">Service for managing meeting-related operations.</param>
    /// <param name="tokenService">Service for handling token-related operations, including user authentication and authorization.</param>
    public InvitationsController(IUserMeetingService userMeetingService, IInvitationsService invitationsService, 
        IMeetingService meetingService, ITokenEntityService tokenService, IUserEntityService userEntityService,
        IChatService chatService)
    {
        _userMeetingService = userMeetingService;
        _invitationsService = invitationsService;
        _meetingService = meetingService;
        _tokenService = tokenService;
        _userEntityService = userEntityService;
        _chatService = chatService;
    }
    
    /// <summary>
    ///     Creates a new invitation for a user to a specified meeting. Can only be performed by the meeting's owner.
    /// </summary>
    /// <remarks>
    ///     This endpoint creates a new invitation for a user to a specified meeting. The action can only be performed by the meeting's owner.
    /// </remarks>
    /// <param name="invitation">An object containing the user ID and meeting ID.</param>
    /// <returns>Details of the created invitation.</returns>
    /// <response code="200">Returns details of the created invitation.</response>
    /// <response code="401">The requesting user is not the owner of the meeting.</response>
    /// <response code="404">The user with the provided ID or the meeting does not exist.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult> CreateInvitation([FromBody]InvitationDTOCreate invitation)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (userId.IsEmpty())
            {
                return NotFound("Requesting user not found.");
            }
            
            var isMeetingOwner = await _meetingService.IsUserAnMeetingOwner(invitation.MeetingId, Guid.Parse(userId));
            if (!isMeetingOwner)
            {
                return Unauthorized("You are not authorized to create this invitation.");
            }
            
            var userInfo = await _userEntityService.GetUserInfo(invitation.UserEmail);
            if (userInfo == null)
            {
                return NotFound("User not found.");
            }
            
            var isUserInvitedAlready = await _invitationsService.IsInvitationExist(userInfo.Id, invitation.MeetingId);
            if (isUserInvitedAlready)
            {
                return NotFound("Invitation already exists.");
            }
                
            var invitationDTO = new InvitationDTO()
            {
                MeetingId = invitation.MeetingId,
                UserId = userInfo.Id
            };
            var newInvitation = await _invitationsService.CreateInvitation(invitationDTO);
            return Ok(newInvitation);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Deletes an invitation for a user to a specified meeting. Can only be performed by the meeting's owner.
    /// </summary>
    /// <remarks>
    ///     This endpoint deletes an existing invitation for a user to a specified meeting. The action can only be performed by the meeting's owner.
    /// </remarks>
    /// <param name="invitationId">The ID of the invitation to be deleted.</param>
    /// <returns>A confirmation message indicating the result of the deletion.</returns>
    /// <response code="200">The invitation was successfully deleted.</response>
    /// <response code="401">The requesting user is not the owner of the meeting.</response>
    /// <response code="404">The invitation, the user, or the meeting does not exist.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpDelete("delete/{invitationId}")]
    [Authorize]
    public async Task<ActionResult> DeleteInvitation(Guid invitationId)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (userId.IsEmpty())
            {
                return NotFound("User not found.");
            }

            var invitation = await _invitationsService.GetInvitationById(invitationId);
            if (invitation == null)
            {
                return NotFound("Invitation not found.");
            }
            
            var isMeetingOwner = await _meetingService.IsUserAnMeetingOwner(invitation.MeetingId, Guid.Parse(userId));
            if (isMeetingOwner == false)
            {
                return Unauthorized("You are not authorized to delete this invitation.");
            }
            await _invitationsService.DeleteInvitation(invitationId);
            return Ok("Deleted successfully");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Confirms an invitation for a user to a specified meeting. Can only be performed by the invited user.
    /// </summary>
    /// <remarks>
    ///     This endpoint confirms an existing invitation and adds the user to the meeting. The action can only be performed by the invited user.
    /// </remarks>
    /// <param name="invitationId">The ID of the invitation to be confirmed.</param>
    /// <returns>A confirmation message indicating the result of the action.</returns>
    /// <response code="200">The invitation was successfully confirmed and the user was added to the meeting.</response>
    /// <response code="401">The requesting user is not the invited user.</response>
    /// <response code="404">The invitation, the user, or the meeting does not exist.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpPost("{invitationId}/confirm")]
    [Authorize]
    public async Task<ActionResult> ConfirmInvitation(Guid invitationId)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (userId.IsEmpty())
            {
                return NotFound("User not found.");
            }
            
            var invitation = await _invitationsService.GetInvitationById(invitationId);
            if (invitation.UserId != Guid.Parse(userId))
            {
                return Unauthorized("You are not authorized to confirm this invitation.");
            }
            
            await _userMeetingService.CreateNewUserMeetingEntity(new UserMeetingDTOCreate()
            {
                UserId = invitation.UserId,
                MeetingId = invitation.MeetingId,
                Status = InvitationStatus.Pending,
                Availability = new byte[10]
            });

            await _chatService.SaveSystemMessageAsync(invitation.MeetingId,
                $"{User.Identity.Name} został dodany do spotkania");
            await _invitationsService.DeleteInvitation(invitationId);
            return Ok("Confirmed successfully");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Declines an invitation for a user to a specified meeting. Can only be performed by the invited user.
    /// </summary>
    /// <remarks>
    ///     This endpoint declines an existing invitation. The action can only be performed by the invited user.
    /// </remarks>
    /// <param name="invitationId">The ID of the invitation to be declined.</param>
    /// <returns>A confirmation message indicating the result of the action.</returns>
    /// <response code="200">The invitation was successfully declined.</response>
    /// <response code="401">The requesting user is not the invited user.</response>
    /// <response code="404">The invitation, the user, or the meeting does not exist.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpDelete("{invitationId}/decline")]
    [Authorize]
    public async Task<ActionResult> DeclineInvitation(Guid invitationId)
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (userId.IsEmpty())
            {
                return NotFound("User not found.");
            }
            
            var invitation = await _invitationsService.GetInvitationById(invitationId);
            if (invitation.UserId != Guid.Parse(userId))
            {
                return Unauthorized("You are not authorized to decline this invitation.");
            }
            await _chatService.SaveSystemMessageAsync(invitation.MeetingId,
                $"{User.Identity.Name} odrzucił zaproszenie do spotkania");
            await _invitationsService.DeleteInvitation(invitationId);
            return Ok("Deleted successfully");
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Returns a list of invitations for a specified user. Can only be performed by the user themselves.
    /// </summary>
    /// <remarks>
    ///     This endpoint retrieves a list of invitations for a specified user. The action can only be performed by the requesting user.
    /// </remarks>
    /// <returns>A list of invitations for the requesting user.</returns>
    /// <response code="200">Returns the list of invitations for the specified user.</response>
    /// <response code="401">The requesting user is not authorized to view these invitations.</response>
    /// <response code="404">The user does not exist.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("user")]
    [Authorize]
   public async Task<ActionResult> GetAllInvitationsByUserId()
{
    try
    {
        var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
        {
            return NotFound("Invalid user ID format.");
        }

        var invitation = await _invitationsService.GetAllInvitationsByUserId(userGuid);
        return Ok(invitation);
    }
    catch (Exception ex)
    {
        // Log the exception
        Console.WriteLine(ex +"Error occurred while getting invitations by user ID");
        return StatusCode(500, "Internal server error");
    }
}

    
    /// <summary>
    ///     Returns a list of invitations for a specified meeting. Can only be performed by a user invited to the meeting.
    /// </summary>
    /// <remarks>
    ///     This endpoint retrieves a list of invitations for a specified meeting. The action can only be performed by a user invited to the meeting.
    /// </remarks>
    /// <param name="meetingId">The ID of the meeting whose invitations are to be retrieved.</param>
    /// <returns>A list of invitations for the specified meeting.</returns>
    /// <response code="200">Returns the list of invitations for the specified meeting.</response>
    /// <response code="401">The requesting user is not authorized to view these invitations.</response>
    /// <response code="404">The meeting or invitations do not exist.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("meeting/{meetingId}")]
    [Authorize]
    public async Task<ActionResult> GetAllInvitationsByMeetingId(Guid meetingId)
    {
        try
        {
            var userIdFromRequestCookie = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (userIdFromRequestCookie.IsEmpty())
            {
                return Unauthorized("User token not valid.");
            }
            
            var invitation = await _invitationsService.GetAllInvitationsByMeetingId(meetingId);
            
            return Ok(invitation);
        }
        catch
        {
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    ///     Returns a list of invitations for a specified user. Can only be performed by the user themselves.
    /// </summary>
    /// <remarks>
    ///     This endpoint retrieves a list of invitations for a specified user. The action can only be performed by the requesting user.
    /// </remarks>
    /// <returns>A list of invitations for the requesting user.</returns>
    /// <response code="200">Returns the list of invitations for the specified user.</response>
    /// <response code="401">The requesting user is not authorized to view these invitations.</response>
    /// <response code="404">The user does not exist.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("invitations")]
    [Authorize]
    public async Task<ActionResult> IsAnyInvitationsByUserId()
    {
        try
        {
            var userId = _tokenService.GetIdFromRequestCookie(HttpContext);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                return NotFound("Invalid user ID format.");
            }

            var invitation = await _invitationsService.GetAllInvitationsByUserId(userGuid);
            if (invitation.Count > 0)
            {
                return Ok(true);
            }
            return Ok(false);
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine(ex +"Error occurred while getting invitations by user ID");
            return StatusCode(500, "Internal server error");
        }
    }
}