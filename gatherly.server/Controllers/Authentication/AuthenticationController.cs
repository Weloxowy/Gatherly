using System.IdentityModel.Tokens.Jwt;
using FluentNHibernate.Conventions;
using gatherly.server.Entities.Authentication;
using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Mailing.MailEntity;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gatherly.server.Controllers.Authentication;

/// <summary>
///     Controller for user authentication operations.
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IBlacklistTokenService _blacklistTokenService;
    private readonly IMailEntityService _mailEntityService;
    private readonly IRecoverySessionService _recoverySessionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISsoSessionService _ssoSessionService;
    private readonly ITokenEntityService _tokenEntityService;
    private readonly IUserEntityService _userService;

    // <summary>
    /// Constructor for AuthenticationController.
    /// </summary>
    /// <param name="userService">Service for user-related operations.</param>
    /// <param name="ssoSessionService">Service for SSO session operations.</param>
    /// <param name="tokenEntityService">Service for token-related operations.</param>
    /// <param name="refreshTokenService">Service for refresh token operations.</param>
    /// <param name="blacklistTokenService">Service for blacklist token operations.</param>
    /// <param name="mailEntityService">Service for mailing operations.</param>
    /// <param name="recoverySessionService">Service for account recovery operations.</param>
    public AuthenticationController(IUserEntityService userService, ISsoSessionService ssoSessionService,
        ITokenEntityService tokenEntityService, IRefreshTokenService refreshTokenService,
        IBlacklistTokenService blacklistTokenService, IMailEntityService mailEntityService,
        IRecoverySessionService recoverySessionService)
    {
        _userService = userService;
        _ssoSessionService = ssoSessionService;
        _tokenEntityService = tokenEntityService;
        _refreshTokenService = refreshTokenService;
        _blacklistTokenService = blacklistTokenService;
        _mailEntityService = mailEntityService;
        _recoverySessionService = recoverySessionService;
    }


    /// <summary>
    ///     Sends a single sign-on (SSO) verification code to the user's email.
    /// </summary>
    /// <remarks>
    ///     This endpoint generates an SSO verification code and sends it to the user's email address for authentication.
    /// </remarks>
    /// <param name="email">User's email address.</param>
    /// <returns>The SSO verification code.</returns>
    /// <response code="200">Returns the SSO verification code.</response>
    /// <response code="400">Invalid request or user data.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [AllowAnonymous]
    [HttpPost("login/code/send")]
    public IActionResult SendSsoCode([FromBody] string email)
    {
        var user = _userService.GetUserInfo(email);
        if (user == null) return NotFound("User profile not found");
        try
        {
            var ssoCode = _ssoSessionService.CreateSso(user.Id, email);
            if (ssoCode == null) return NotFound("User profile not found");
            _mailEntityService.SendSsoCodeEmailAsync(user, ssoCode);
            return Ok("Email was send");
        }
        catch (Exception exception)
        {
            return StatusCode(500, exception.Message);
            //return StatusCode(500, "There was a problem while creating a SSO token. Please try again later");
        }
    }

    /// <summary>
    ///     Verifies the single sign-on (SSO) verification code and authenticates the user.
    /// </summary>
    /// <remarks>
    ///     This endpoint verifies the provided SSO verification code and authenticates the user if valid.
    /// </remarks>
    /// <param name="data">User login data including email and SSO code.</param>
    /// <returns>Authentication tokens upon successful verification.</returns>
    /// <response code="200">Returns the authentication tokens.</response>
    /// <response code="400">Invalid request or incorrect SSO code.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [AllowAnonymous]
    [HttpPost("login/code/verify")]
    public IActionResult VerifySsoCode([FromBody] UserEntityDTOLoginCode data)
    {
        var user = _userService.GetUserInfo(data.Email);
        if (user == null) return NotFound("User profile not found");

        var ssoSession = _ssoSessionService.ValidSso(user.Id, data.Code);
        if (!ssoSession) return BadRequest("Wrong SSO code");
        try
        {
            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id);
            var jwt = _tokenEntityService.GenerateToken(user, refresh.Id.ToString());

            var jwtCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1) // Czas ważności refresh tokenu
            };

            Response.Cookies.Append("Authorization", "Bearer " + jwt, jwtCookieOptions);
            Response.Cookies.Append("RefreshToken", refresh.Token, refreshCookieOptions);

            return Ok("Login successfully");
        }
        catch
        {
            return StatusCode(500, "There was a problem while creating a SSO token. Please try again later");
        }
    }

    /// <summary>
    ///     Authenticates the user using standard username-password credentials.
    /// </summary>
    /// <remarks>
    ///     This endpoint verifies the user credentials and provides authentication tokens upon successful validation.
    /// </remarks>
    /// <param name="data">User login data including email and password.</param>
    /// <returns>Authentication tokens upon successful authentication.</returns>
    /// <response code="200">Returns the authentication tokens.</response>
    /// <response code="400">Invalid request or incorrect credentials.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [AllowAnonymous]
    [HttpPost("login/standard/verify")]
    public IActionResult LoginUserByPassword([FromBody] UserEntityDTOLoginPassword data)
    {
        var isUser = _userService.IsUserExists(data.Email);
        if (isUser == false) return NotFound("User profile not found");
        var user = _userService.VerifyUser(data);
        if (user == null) return Unauthorized("Credentials are invalid");
        try
        {
            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id);
            var jwt = _tokenEntityService.GenerateToken(user, refresh.Id.ToString());

            var jwtCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("Authorization", "Bearer " + jwt, jwtCookieOptions);
            Response.Cookies.Append("RefreshToken", refresh.Token, refreshCookieOptions);

            return Ok("Login successfully");
        }
        catch
        {
            return StatusCode(500, "There was a problem while proceeding a SSO token. Please try again later");
        }
    }

    /// <summary>
    ///     Registers a new user.
    /// </summary>
    /// <remarks>
    ///     This endpoint creates a new user based on the provided registration data and provides authentication tokens upon
    ///     successful registration.
    /// </remarks>
    /// <param name="data">User registration data.</param>
    /// <returns>Authentication tokens upon successful registration.</returns>
    /// <response code="200">Returns the authentication tokens.</response>
    /// <response code="400">Invalid request or user data.</response>
    /// <response code="500">Internal server error occurred.</response>
    [AllowAnonymous]
    [HttpPost("register")]
    public ActionResult<UserEntity> CreateNewUser([FromBody] UserEntityDTOCreate data)
    {
        var user = _userService.GetUserInfo(data.Email);
        if (user != null) return BadRequest("Email address is already used");

        var newUser = _userService.CreateNewUser(data);
        if (newUser == null) return StatusCode(500, "Wystąpił błąd podczas tworzenia użytkownika");

        var refresh = _refreshTokenService.GenerateRefreshToken(newUser.Id);
        var jwt = _tokenEntityService.GenerateToken(newUser, refresh.Id.ToString());

        var jwtCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        Response.Cookies.Append("Authorization", "Bearer " + jwt, jwtCookieOptions);
        Response.Cookies.Append("RefreshToken", refresh.Token, refreshCookieOptions);

        return Ok("Login successfully");
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var mail = _tokenEntityService.GetEmailFromRequestCookie(HttpContext);
        if (mail.IsEmpty()) return Unauthorized("User profile not found");
        var user = _userService.GetUserInfo(mail);
        if (user == null) return NotFound("User profile not found");

        var authorizationHeader = HttpContext.Request.Cookies["Authorization"];
        if (authorizationHeader == null || authorizationHeader.StartsWith("Bearer "))
        {
            Response.Cookies.Delete("Authorization");
        }

        var refreshTokenHeader = HttpContext.Request.Cookies["RefreshToken"];
        if (refreshTokenHeader != null)
        {
            var refreshToken = refreshTokenHeader.Trim();
            _blacklistTokenService.AddToBlacklist(refreshToken, user.Id, DateTime.Now.AddHours(2));
            _refreshTokenService.RevokeRefreshToken(refreshToken);
            Response.Cookies.Delete("RefreshToken");
        }
        
        return Ok("User logged out successfully.");
    }

    //reset - not tested

    /// <summary>
    ///     Sends a request to user mail to reset password.
    /// </summary>
    /// <remarks>
    ///     This endpoint generates an account recovery session and sends email message with link to authorize change.
    /// </remarks>
    /// <param name="email">User's email address.</param>
    /// <returns>Ok return message</returns>
    /// <response code="200">Returns the OK message.</response>
    /// <response code="400">Invalid request or user data.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [AllowAnonymous]
    [HttpPost("recover/send")]
    public IActionResult SendRecover([FromBody] string email)
    {
        var user = _userService.GetUserInfo(email);
        if (user == null) return NotFound("User profile not found");
        try
        {
            var session = _recoverySessionService.CreateSession(user.Id, email);
            if (session == null)
                return NotFound("There was a problem while creating a recovery link. Please try again later");
            _mailEntityService.SendRecoveryEmailAsync(user, session);
            return Ok("Email was send succesfully");
        }
        catch
        {
            //return StatusCode(500, exception.Message);
            return StatusCode(500, "There was a problem while creating a recovery link. Please try again later");
        }
    }

    /// <summary>
    ///     Validates if the request is correct.
    /// </summary>
    /// <remarks>
    ///     This endpoint checks is the recovery link is valid.
    /// </remarks>
    /// <param name="id">Id of recovery session.</param>
    /// <response code="200">Returns OK message.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [AllowAnonymous]
    [HttpGet("recover/validate/{id}")]
    public IActionResult ValidRecover(string id)
    {
        try
        {
            var isValid = _recoverySessionService.OpenRecoverySession(Guid.Parse(id)); //isOpened = true
            if (isValid == false) return NotFound("Active recovery session not found");
            return Ok();
            //return Redirect("https://localhost:3000/auth/recovery/"+id);
        }
        catch
        {
            return StatusCode(500, "There was a problem while validate a recovery session. Please try again");
        }
    }

    /// <summary>
    ///     Changes the user password.
    /// </summary>
    /// <remarks>
    ///     This endpoint changes password if the link is valid and procedure is opened/
    /// </remarks>
    /// <param name="data">User email, old and new password.</param>
    /// <response code="200">Returns the Ok message.</response>
    /// <response code="400">Function was used without ValidateRecovery endpoint first.</response>
    /// <response code="404">User with the provided email does not exist.</response>
    /// <response code="500">Internal server error occurred.</response>
    [AllowAnonymous]
    [HttpPost("recover/change")]
    public IActionResult ApplyNewPasswordRecover([FromBody] UserEntityDTOResetPassword data)
    {
        try
        {
            var session = _recoverySessionService.CloseRecoverySession(data.Email);
            if (session == false) return NotFound("Active recovery session not found");

            var changePassword = _userService.ChangeUserPassword(data);
            return changePassword == null
                ? StatusCode(500, "There was a problem while recovering your account. Please try again later")
                : Ok("Recovery was successful");
        }
        catch
        {
            return StatusCode(500, "There was a problem while recovering your account. Please try again later");
        }
    }
}