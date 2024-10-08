﻿using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using gatherly.server.Persistence.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gatherly.server.Controllers.Authentication;

/// <summary>
///     Controller for managing user profiles and operations.
/// </summary>
[Route("/api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ITokenEntityService _tokenEntityService;
    private readonly IUserEntityService _userService;
    private readonly TokenHelper _tokenHelper;

    /// <summary>
    ///     Constructor for UserController.
    /// </summary>
    /// <param name="userService">Service for user-related operations.</param>
    /// <param name="tokenEntityService">Service for token-related operations.</param>
    /// <param name="refreshTokenService">Service for refresh token operations.</param>
    public UserController(IUserEntityService userService, ITokenEntityService tokenEntityService,
        TokenHelper tokenHelper)
    {
        _userService = userService;
        _tokenEntityService = tokenEntityService;
        _tokenHelper = tokenHelper;
    }
    
    //operations for logged-in user (not Admin)

    /// <summary>
    ///     Retrieves the profile information of the currently logged-in user.
    /// </summary>
    /// <remarks>
    ///     This endpoint retrieves the profile information of the currently logged-in user using the JWT token stored in the
    ///     request header.
    /// </remarks>
    /// <returns>The profile information of the logged-in user.</returns>
    /// <response code="200">Returns the user profile information.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">Error occurred while accessing the user profile.</response>
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserEntity>> GetLoggedInUserProfile()
    {

        var mail = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (mail == null) return Unauthorized("You have no access to this resource");

        var user = await _userService.GetUserInfo(mail);
        return user == null ? StatusCode(500, "There was a problem while accessing the record. Please try again later") : Ok(user.ToDto());
    }

    /// <summary>
    ///     Updates the profile information of the currently logged-in user.
    /// </summary>
    /// <remarks>
    ///     This endpoint updates the profile information of the currently logged-in user based on the provided data.
    /// </remarks>
    /// <param name="newData">Updated profile data.</param>
    /// <returns>The updated profile information of the logged-in user.</returns>
    /// <response code="200">Returns the updated user profile information.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">Error occurred while updating the user profile.</response>
    [Authorize]
    [HttpPatch("profile")]
    public async Task<ActionResult<UserEntity>> UpdateLoggedInUserProfile([FromBody] UserEntityDTOUpdate newData)
    {
        var mail = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (mail == null) return Unauthorized("You have no access to this resource");

        var user = await _userService.PatchUserInfo(newData, mail);
        if (user == null)
            return StatusCode(500, "There was a problem while modifying the data. Please try again later");

        var tokens = _tokenHelper.GenerateTokens(user);
        CookieHelper.SetJwtCookie(Response, tokens.JwtToken);
        CookieHelper.SetRefreshTokenCookie(Response, tokens.RefreshToken);

        return Ok(user.ToDto());
    }


    /// <summary>
    ///     Deletes the profile information of the currently logged-in user.
    /// </summary>
    /// <remarks>
    ///     This endpoint deletes the profile information of the currently logged-in user.
    /// </remarks>
    /// <returns>Message indicating successful deletion of user profile.</returns>
    /// <response code="200">Returns a success message confirming deletion.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">Error occurred while deleting the user profile.</response>
    [Authorize]
    [HttpDelete("profile")]
    public async Task<ActionResult<UserEntity>> DeleteUser()
    {
        var mail = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (mail == null) return Unauthorized("You have no access to this resource");

        var result = await _userService.DeleteUserInfo(mail);

        return !result ? StatusCode(500, "There was a problem while deleting the user. Please try again later") : Ok("User deleted");
    }
/*
    //operations for admin - not in use

    /// <summary>
    ///     Retrieves the profile information of a user by their ID.
    /// </summary>
    /// <remarks>
    ///     This endpoint retrieves the profile information of a user by their ID, accessible only by administrators.
    /// </remarks>
    /// <param name="id">User ID.</param>
    /// <returns>The profile information of the user.</returns>
    /// <response code="200">Returns the user profile information.</response>
    /// <response code="401">User is not authenticated or not authorized to access this resource.</response>
    /// <response code="404">User profile not found.</response>
    /// <response code="500">Error occurred while accessing the user profile.</response>
    [Authorize]
    [HttpGet("profile/{id}")]
    public async Task<ActionResult<UserEntity>> GetUserById(Guid id)
    {
        var mail = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (mail == null) return Unauthorized("You have an invalid access token");

        var isAdmin = await _userService.IsUserAdmin(mail);
        if (isAdmin != true) return Unauthorized("You have no access to this resource");

        var user = await _userService.GetUserInfo(id);
        if (user == null)
            return StatusCode(500, "There was a problem while accessing the data. Please try again later");
        return Ok(user.ToDto());
    }


    /// <summary>
    ///     Updates the profile information of a user by their ID.
    /// </summary>
    /// <remarks>
    ///     This endpoint updates the profile information of a user by their ID, accessible only by administrators.
    /// </remarks>
    /// <param name="newData">Updated profile data.</param>
    /// <param name="id">User ID.</param>
    /// <returns>The updated profile information of the user.</returns>
    /// <response code="200">Returns the updated user profile information.</response>
    /// <response code="401">User is not authenticated or not authorized to access this resource.</response>
    /// <response code="404">User profile not found.</response>
    /// <response code="500">Error occurred while updating the user profile.</response>
    [Authorize]
    [HttpPatch("profile/{id}")]
    public async Task<ActionResult<UserEntity>> PatchUserById([FromBody] UserEntityDTOUpdate newData, Guid id)
    {
        var mail = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (mail == null) return Unauthorized("You have an invalid token");

        var isAdmin = await _userService.IsUserAdmin(mail);
        if (isAdmin != true) return Unauthorized("You have no access to this resource");

        var userEmail = await _userService.GetUserInfo(id);
        if (userEmail == null) return NotFound("User profile not found");

        var user = await _userService.PatchUserInfo(newData, userEmail.Email);
        if (user == null) return StatusCode(500, "There was a problem while updating the data. Please try again later");

        return Ok(user.ToDto());
    }


    /// <summary>
    ///     Deletes the profile information of a user by their ID.
    /// </summary>
    /// <remarks>
    ///     This endpoint deletes the profile information of a user by their ID, accessible only by administrators.
    /// </remarks>
    /// <param name="id">User ID.</param>
    /// <returns>Message indicating successful deletion of user profile.</returns>
    /// <response code="200">Returns a success message confirming deletion.</response>
    /// <response code="401">User is not authenticated or not authorized to access this resource.</response>
    /// <response code="404">User profile not found.</response>
    /// <response code="500">Error occurred while deleting the user profile.</response>
    [Authorize]
    [HttpDelete("profile/{id}")]
    public async Task<ActionResult<UserEntity>> DeleteExistingUser(string id)
    {
        var mail = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (mail == null) return Unauthorized("You have an invalid token");

        var isAdmin = await _userService.IsUserAdmin(mail);
        if (isAdmin != true) return Unauthorized("You have no access to this resource");

        var userEmail = await _userService.GetUserInfo(id);
        if (userEmail == null) return NotFound("User profile not found");

        var result = await _userService.DeleteUserInfo(userEmail.Email);
        if (!result)
            return StatusCode(500, "There was a problem while deleting the user. Please try again later");
        return Ok("User deleted");
    }
*/   
}