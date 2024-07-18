using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DotNetEnv;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace gatherly.server.Controllers.Tokens;

[Route("api/[controller]")]
[ApiController]
public class TokensController : ControllerBase
{
    private readonly IBlacklistTokenService _blacklistTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ITokenEntityService _tokenEntityService;
    private readonly IUserEntityService _userEntityService;

    public TokensController(ITokenEntityService tokenEntityService, IRefreshTokenService refreshTokenService,
        IUserEntityService userEntityService, IBlacklistTokenService blacklistTokenService)
    {
        _tokenEntityService = tokenEntityService;
        _refreshTokenService = refreshTokenService;
        _userEntityService = userEntityService;
        _blacklistTokenService = blacklistTokenService;
    }

    /// <summary>
    ///     Issues a new JWT.
    /// </summary>
    /// <param name="request">The refresh request containing the refresh token.</param>
    /// <returns>Returns a new JWT if the refresh token is valid; otherwise, returns Unauthorized.</returns>
    /// <response code="200">Returns the authentication tokens.</response>
    /// <response code="401">Incorrect credentials or expired token.</response>
    /// <response code="404">Incorrect credentials.</response>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var refresh = await _refreshTokenService.GetRefreshToken(request.RefreshToken);

            if (refresh == null || refresh.Expiration <= DateTime.Now) return Unauthorized();

            var user = _userEntityService.GetUserInfo(refresh.UserId);
            if (user == null) return NotFound("Incorect user data");

            var token = _tokenEntityService.GenerateToken(user, refresh.Id.ToString());

            return Ok(new { token });
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
        var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
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
                ValidIssuer = "localhost:44329",
                ValidAudience = "localhost:3000",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("SECRET")))
            };

            var principal = handler.ValidateToken(jwtToken, validationParameters, out var validatedToken);

            var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti)) return BadRequest("JWT ID (jti) not found in the token.");

            var isBlacklisted = _blacklistTokenService.IsTokenBlacklisted(jti);
            if (isBlacklisted != null) return Unauthorized("The token has been blacklisted.");

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
        var authorizationHeader = Request.Headers["Refresh"].FirstOrDefault().Trim();
        if (string.IsNullOrEmpty(authorizationHeader))
            return BadRequest("Refresh Token is not present in the request header.");

        try
        {
            var isBlacklisted = _blacklistTokenService.IsTokenBlacklisted(authorizationHeader);
            if (isBlacklisted != null) return Unauthorized("The token has been blacklisted.");

            var refreshToken = _refreshTokenService.GetRefreshToken(authorizationHeader);
            if (refreshToken == null || refreshToken.Result.IsRevoked)
                return BadRequest("The refresh token was not found or is inactive. Check the token status.");

            return Ok($"The token is valid until {refreshToken.Result.Expiration}");
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
        var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            return BadRequest("JWT Token is not present in the request header.");

        var jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            var jti = jsonToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti)) return BadRequest("Cannot extract JWT ID (jti) from the token.");

            await _refreshTokenService.RevokeRefreshToken(jti);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while revoking the JWT: " + ex.Message);
        }
    }
}