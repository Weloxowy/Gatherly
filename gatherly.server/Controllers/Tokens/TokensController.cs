using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DotNetEnv;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace gatherly.server.Controllers.Tokens;

/// <summary>
///     Controller responsible for handling operations related to tokens.
/// </summary>
/// <remarks>
///     This controller provides endpoints for managing tokens, such as issuing, refreshing, and invalidating tokens.
///     It also includes operations related to blacklisting tokens.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class TokensController : ControllerBase
{
    private readonly IBlacklistTokenService _blacklistTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ITokenEntityService _tokenEntityService;
    private readonly IUserEntityService _userEntityService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TokensController"/> class.
    /// </summary>
    /// <param name="tokenEntityService">Service for token-related operations.</param>
    /// <param name="refreshTokenService">Service for refresh token operations.</param>
    /// <param name="userEntityService">Service for user-related operations.</param>
    /// <param name="blacklistTokenService">Service for managing blacklisted tokens.</param>
    public TokensController(ITokenEntityService tokenEntityService, IRefreshTokenService refreshTokenService,
        IUserEntityService userEntityService, IBlacklistTokenService blacklistTokenService)
    {
        _tokenEntityService = tokenEntityService;
        _refreshTokenService = refreshTokenService;
        _userEntityService = userEntityService;
        _blacklistTokenService = blacklistTokenService;
    }

    /// <summary>
    ///     Issues a new JWT and Refresh tokens.
    /// </summary>
    /// <returns>Returns a new JWT if the refresh token is valid; otherwise, returns Unauthorized.</returns>
    /// <response code="200">Returns the authentication tokens.</response>
    /// <response code="401">Incorrect credentials or expired token.</response>
    /// <response code="404">Incorrect credentials.</response>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokens()
    {
        var refreshToken = Request.Cookies["RefreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest("Refresh Token is not present in the request cookies.");
        refreshToken = refreshToken.Trim();

        try
        {
            var isBlacklisted = _blacklistTokenService.IsTokenBlacklisted(refreshToken);
            if (isBlacklisted) return Unauthorized("The token has been blacklisted.");

            var oldRefreshToken = _refreshTokenService.GetRefreshToken(refreshToken);
            if (oldRefreshToken == null)
                return BadRequest("The refresh token was not found or is inactive. Check the token status.");

            var userId = oldRefreshToken.UserId;
            var userEntity = _userEntityService.GetUserInfo(userId);
            if (userEntity == null) return NotFound("User not found.");

            var newRefreshToken = _refreshTokenService.GenerateRefreshToken(userId);
            var newJwtToken = _tokenEntityService.GenerateToken(userEntity, newRefreshToken.Id.ToString());

            var jwtCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("Authorization", "Bearer " + newJwtToken, jwtCookieOptions);
            Response.Cookies.Append("RefreshToken", newRefreshToken.Token, refreshCookieOptions);

            return Ok("The tokens are refreshed");
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized("The refresh token is invalid or expired: " + ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while validating the refresh token: " + ex.Message);
        }
    }


    /// <summary>
    ///     Validates a JWT.
    /// </summary>
    /// <returns>Returns OK if the JWT is valid; otherwise, returns an error status.</returns>
    /// <response code="200">Returns the time of expiration for JWT token.</response>
    /// <response code="400">Token is not provided in the request.</response>
    /// <response code="401">Incorrect credentials or expired token.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("jwt/validate")]
    public async Task<IActionResult> ValidateJwtToken()
    {
        var authorizationHeader = Request.Cookies["Authorization"];
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            return BadRequest("JWT Token is not present in the request header.");

        var jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                //ValidIssuer = "https://gatherly.azurewebsites.net",
                ValidIssuer = "https://localhost:44329",
                //ValidAudience = "https://gatherly-mocha.vercel.app",
                ValidAudience = "https://localhost:3000",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("SECRET")))
            };

            var principal = handler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

            var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti)) return BadRequest("JWT ID (jti) not found in the token.");

            var isBlacklisted = _blacklistTokenService.IsTokenBlacklisted(jti);
            if (isBlacklisted) return Unauthorized("The token has been blacklisted.");

            var expiration = jsonToken.ValidTo;
            if (expiration <= DateTime.UtcNow) return Unauthorized("The JWT has expired.");

            return Ok($"The token is valid until {expiration}");
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized("The JWT is invalid or expired: " + ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while validating the JWT: " + ex.Message);
        }
    }

    /// <summary>
    ///     Validates a refresh token.
    /// </summary>
    /// <returns>Returns OK if the refresh token is valid; otherwise, returns an error status.</returns>
    /// <response code="200">Returns the time of expiration for refresh token.</response>
    /// <response code="400">Refresh token is not provided in the request.</response>
    /// <response code="401">Incorrect or expired token.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("refresh/validate")]
    public async Task<IActionResult> ValidateRefreshToken()
    {
        var authorizationHeader = Request.Cookies["RefreshToken"];
        if (string.IsNullOrEmpty(authorizationHeader))
            return BadRequest("Refresh Token is not present in the request header.");
        authorizationHeader = authorizationHeader.Trim();
        try
        {
            var isBlacklisted = _blacklistTokenService.IsTokenBlacklisted(authorizationHeader);
            if (isBlacklisted) return Unauthorized("The token has been blacklisted.");
            //problem z async
            var refreshToken = _refreshTokenService.GetRefreshToken(authorizationHeader);
            if (refreshToken == null)
                return BadRequest("The refresh token was not found or is inactive. Check the token status.");

            return Ok($"The token is valid until {refreshToken.Expiration}");
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized("The refresh token is invalid or expired: " + ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while validating the refresh token: " + ex.Message);
        }
    }

    /// <summary>
    ///     Revokes both JWT and refresh tokens.
    /// </summary>
    /// <returns>Returns OK if the tokens are successfully revoked; otherwise, returns an error status.</returns>
    /// <response code="200">Tokens revoked successfully.</response>
    /// <response code="400">JWT token is not provided in the request.</response>
    /// <response code="401">Incorrect or expired token.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken()
    {
        var authToken = Request.Cookies["Authorization"];
        var refreshToken = Request.Cookies["RefreshToken"];
        
        if (string.IsNullOrEmpty(authToken) && string.IsNullOrEmpty(refreshToken))
            return BadRequest("JWT and Refresh Tokens are not present in the request cookies.");

        if (!string.IsNullOrEmpty(authToken) && authToken.StartsWith("Bearer "))
        {
            var jwtToken = authToken.Substring("Bearer ".Length).Trim();
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

                var userEmail = jsonToken.Subject;
                if (userEmail.IsNullOrEmpty())
                    return BadRequest("User id is incorrect. Check if the account wasnt deleted");
                
                var user = _userEntityService.GetUserInfo(userEmail);
                
                var jti = jsonToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
                
                if (string.IsNullOrEmpty(jti))
                    return BadRequest("Cannot extract JWT ID (jti) from the token.");
                
                await _refreshTokenService.RevokeRefreshToken(jti);
                
                _blacklistTokenService.AddToBlacklist(jwtToken, user.Id, DateTime.UtcNow.AddHours(2));
                _blacklistTokenService.AddToBlacklist(jti, user.Id, DateTime.UtcNow.AddHours(2));
                
                Response.Cookies.Delete("Authorization");
                Response.Cookies.Delete("RefreshToken");

                return Ok("Tokens revoked successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while revoking the JWT: " + ex.Message);
            }
        }

        if (!string.IsNullOrEmpty(refreshToken))
        {
            try
            {
                var oldRefreshToken = _refreshTokenService.GetRefreshToken(refreshToken);
                if (oldRefreshToken == null)
                    return BadRequest("The refresh token was not found or is inactive. Check the token status.");

                var userId = oldRefreshToken.UserId;
                await _refreshTokenService.RevokeRefreshToken(refreshToken);
                _blacklistTokenService.AddToBlacklist(refreshToken, userId, DateTime.UtcNow.AddHours(2));

                Response.Cookies.Delete("Authorization");
                Response.Cookies.Delete("RefreshToken");

                return Ok("Tokens revoked successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while revoking the refresh token: " + ex.Message);
            }
        }
        return BadRequest("JWT and Refresh Tokens are not present in the request cookies.");
    }
}