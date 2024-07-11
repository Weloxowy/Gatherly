using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using gatherly.server.Entities.Authentication;
using gatherly.server.Entities.Tokens;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace gatherly.server.Controllers.Authentication;

/// <summary>
/// Controller for user authentication operations.
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IUserEntityService _userService;
    private readonly ISsoSessionService _ssoSessionService;
    private readonly ITokenEntityService _tokenEntityService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IBlacklistTokenService _blacklistTokenService;

    // <summary>
    /// Constructor for AuthenticationController.
    /// </summary>
    /// <param name="userService">Service for user-related operations.</param>
    /// <param name="ssoSessionService">Service for SSO session operations.</param>
    /// <param name="tokenEntityService">Service for token-related operations.</param>
    /// <param name="refreshTokenService">Service for refresh token operations.</param>
    /// <param name="blacklistTokenService">Service for blacklist token operations.</param>
    public AuthenticationController(IUserEntityService userService, ISsoSessionService ssoSessionService,
        ITokenEntityService tokenEntityService, IRefreshTokenService refreshTokenService,
        IBlacklistTokenService blacklistTokenService)
    {
        _userService = userService;
        _ssoSessionService = ssoSessionService;
        _tokenEntityService = tokenEntityService;
        _refreshTokenService = refreshTokenService;
        _blacklistTokenService = blacklistTokenService;
    }
    
  
    /// <summary>
    /// Sends a single sign-on (SSO) verification code to the user's email.
    /// </summary>
    /// <remarks>
    /// This endpoint generates an SSO verification code and sends it to the user's email address for authentication.
    /// </remarks>
    /// <param name="email">User's email address.</param>
    /// <returns>The SSO verification code.</returns>
    /// <response code="200">Returns the SSO verification code.</response>
    /// <response code="400">Invalid request or user data.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost("login/code/send")]
    public IActionResult SendSsoCode([FromBody] string email)
    {
        var user = _userService.GetUserInfo(email);
        if (user == null)
        {
            return NotFound("User profile not found");
        }
        try
        {
            var ssoCode = _ssoSessionService.CreateSso(user.Id, email);
            // Send email
            return Ok(new { SSOCode = ssoCode.VerificationCode }); //only for development
        }
        catch
        {
            return StatusCode(500, "There was a problem while creating a SSO token. Please try again later");
        }
    }


    /// <summary>
    /// Verifies the single sign-on (SSO) verification code and authenticates the user.
    /// </summary>
    /// <remarks>
    /// This endpoint verifies the provided SSO verification code and authenticates the user if valid.
    /// </remarks>
    /// <param name="data">User login data including email and SSO code.</param>
    /// <returns>Authentication tokens upon successful verification.</returns>
    /// <response code="200">Returns the authentication tokens.</response>
    /// <response code="400">Invalid request or incorrect SSO code.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost("login/code/verify")]
    public IActionResult VerifySsoCode([FromBody] UserEntityDTOLoginCode data)
    {
        var user = _userService.GetUserInfo(data.Email);
        if (user == null)
        {
            return NotFound("User profile not found");
        }
            
        var ssoSession = _ssoSessionService.ValidSso(user.Id, data.Code);
        if (!ssoSession)
        {
            return BadRequest("Wrong SSO code");
        }
        try
        {
            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id);
            var jwt = _tokenEntityService.GenerateToken(user, refresh.Id.ToString());
            
            Response.Headers.Append("Authorization", $"Bearer {jwt}");
            Response.Headers.Append("RefreshToken", refresh.Token);
            
            return Ok($"User {user.Email} is authorized. {refresh.Token}, {jwt}");
        }
        catch
        {
            return StatusCode(500, "There was a problem while creating a SSO token. Please try again later");
        }
    }
    
    /// <summary>
    /// Authenticates the user using standard username-password credentials.
    /// </summary>
    /// <remarks>
    /// This endpoint verifies the user credentials and provides authentication tokens upon successful validation.
    /// </remarks>
    /// <param name="data">User login data including email and password.</param>
    /// <returns>Authentication tokens upon successful authentication.</returns>
    /// <response code="200">Returns the authentication tokens.</response>
    /// <response code="400">Invalid request or incorrect credentials.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost("login/standard/verify")]
    public IActionResult LoginUserByPassword([FromBody] UserEntityDTOLoginPassword data)
    {
        var user = _userService.VerifyUser(data);
        if (user == null)
        {
            return NotFound("User profile not found");
        }
        try
        {
            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id);
            var jwt = _tokenEntityService.GenerateToken(user, refresh.Id.ToString());
            
            Response.Headers.Append("Authorization", $"Bearer {jwt}");
            Response.Headers.Append("RefreshToken", refresh.Token);
            
            return Ok($"User {user.Email} is authorized. {refresh.Token}, {jwt}");
        }
        catch
        {
            return StatusCode(500, "There was a problem while proceeding a SSO token. Please try again later");
        }
    }
   
    // <summary>
    /// Registers a new user.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new user based on the provided registration data and provides authentication tokens upon successful registration.
    /// </remarks>
    /// <param name="data">User registration data.</param>
    /// <returns>Authentication tokens upon successful registration.</returns>
    /// <response code="200">Returns the authentication tokens.</response>
    /// <response code="400">Invalid request or user data.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost("register")]
    public ActionResult<UserEntity> CreateNewUser([FromBody] UserEntityDTOCreate data)
    {
        var user = _userService.GetUserInfo(data.Email);
        if (user != null)
        {
            return BadRequest("Email address is already used");
        }
        
        var newUser = _userService.CreateNewUser(data);
        if (newUser == null)
        {
            return StatusCode(500, "Wystąpił błąd podczas tworzenia użytkownika");
        }
        
        var refresh = _refreshTokenService.GenerateRefreshToken(newUser.Id); 
        var jwt = _tokenEntityService.GenerateToken(newUser, refresh.Id.ToString());
        
        Response.Headers.Append("Authorization", $"Bearer {jwt}");
        Response.Headers.Append("RefreshToken", refresh.Token);
                        
        return Ok($"User {newUser.Email} is registered and authorized. {refresh.Token}, {jwt}");
    }
    
    /*
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var mail = _tokenEntityService.GetEmailFromRequestHeader(HttpContext);
        if (mail == null)
        {
            return Unauthorized();
        }
        var user = _userService.GetUserInfo(mail);
        if (user == null)
        {
            return NotFound("User profile not found");
        }

        var authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
        {
            return Unauthorized();
        }
    
        var jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();

        var refreshTokenHeader = HttpContext.Request.Headers["RefreshToken"].FirstOrDefault();
        if (refreshTokenHeader == null)
        {
            return Unauthorized();
        }
    
        var refreshToken = refreshTokenHeader.Trim();

        _blacklistTokenService.AddToBlacklist(jwtToken,user.Id,DateTime.Now.AddHours(2));
        _blacklistTokenService.AddToBlacklist(refreshToken,user.Id,DateTime.Now.AddHours(2));

        _refreshTokenService.RevokeRefreshToken(refreshToken);

        Response.Headers.Remove("Authorization");
        Response.Headers.Remove("RefreshToken");
    
        return Ok("User logged out successfully.");
    }
    */
}