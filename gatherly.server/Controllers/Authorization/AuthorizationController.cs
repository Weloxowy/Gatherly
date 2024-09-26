using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace gatherly.server.Controllers.Authorization;

/// <summary>
///     Controller responsible for handling user authorization and role validation.
/// </summary>
/// <remarks>
///     This controller provides endpoints for checking user roles and permissions, 
///     including verifying if a user is valid, checking if a user is an admin, 
///     and changing the role of a user.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly ITokenEntityService _tokenEntityService;
    private readonly IUserEntityService _userService;
    
    /// <summary>
    ///     Constructor for AuthorizationController.
    /// </summary>
    /// <param name="userService">Service for user-related operations.</param>
    /// <param name="tokenEntityService">Service for token-related operations.</param>
    public AuthorizationController(IUserEntityService userService, ITokenEntityService tokenEntityService)
    {
        _userService = userService;
        _tokenEntityService = tokenEntityService;
    }
    
    /// <summary>
    ///     Validates if the current user is a valid user.
    /// </summary>
    /// <remarks>
    ///     This endpoint checks if the current user's email exists in the system.
    /// </remarks>
    /// <returns>
    ///     Returns info if the user exists.
    /// </returns>
    /// <response code="200">Returns OK if the user exists.</response>
    /// <response code="404">Returns Not Found if the user does not exist.</response>
    [HttpPost("isUser")]
    [Authorize]
    public async Task<ActionResult> IsValidUser()
    {
        var email = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (email.Equals(null))
        {
            return NotFound("User not found");
        }
        return await _userService.IsUserExists(email) == false ? NotFound("User not found") : Ok("User exists");
    }
    
    /// <summary>
    ///     Validates if the current user is an administrator.
    /// </summary>
    /// <remarks>
    ///     This endpoint checks if the current user's email corresponds to an administrator.
    /// </remarks>
    /// <returns>
    ///     Returns user status.
    /// </returns>
    /// <response code="200">User is an admin.</response>
    /// <response code="404">User is not an admin or does not exist.</response>
    [HttpGet("isAdmin")]
    [Authorize]
    public async Task<ActionResult> IsValidAdmin()
    {
        var email = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (email.Equals(null))
        {
            return NotFound("User not found");
        }
        return await _userService.IsUserAdmin(email) == false ? NotFound("User is not an admin or doesnt exist") : Ok("User is an admin");
    }
    
    /// <summary>
    ///     Changes the role of a specified user.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows an admin to change the role of a user to either 'User' or 'Admin'.
    /// </remarks>
    /// <param name="userId">The ID of the user whose role is to be changed.</param>
    /// <returns>
    ///     Returns status with a message indicating the new role of the user.
    /// </returns>
    /// <response code="200">Returns OK with the new role status of the user.</response>
    /// <response code="401">Requester is not an admin.</response>
    /// <response code="404">User does not exist.</response>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> ChangeUserRole(Guid userId)
    {
        var email = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (email.Equals(null))
        {
            return NotFound("User not found");
        }

        if (await _userService.IsUserAdmin(email) == false)
        {
            return Unauthorized("You are not an admin.");
        }
        var user = await _userService.ChangeUserStatus(userId);
        return user.UserRole == UserRole.User ? Ok("Account has now user status") : Ok("Account has now admin status");
    }

    
}