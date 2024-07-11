using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using gatherly.server.Persistence.Tokens.TokenEntity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace gatherly.server.Controllers.Tokens;

[Route("api/[controller]")]
[ApiController]
public class TokensController : ControllerBase
{
    private readonly ITokenEntityService _tokenEntityService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserEntityService _userEntityService;

    public TokensController(ITokenEntityService tokenEntityService, IRefreshTokenService refreshTokenService, IUserEntityService userEntityService)
    {
        _tokenEntityService = tokenEntityService;
        _refreshTokenService = refreshTokenService;
        _userEntityService = userEntityService;
    }
    
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        
        using (var session = NHibernateHelper.OpenSession())
        {
            var refresh = _refreshTokenService.GetRefreshToken(request.RefreshToken).Result;
            
            if (refresh == null || refresh.Expiration <= DateTime.Now)
            {
                return Unauthorized();
            }

            var user = _userEntityService.GetUserInfo(refresh.UserId);
            if (user == null)
            {
                return Unauthorized();
            }
            
            var token = _tokenEntityService.GenerateToken(user,refresh.Id.ToString());
            
            return Ok(new { token });
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateToken()
    {
        // Wydobycie jwt_token z nagłówka cookie
        var jwtToken = Request.Cookies["jwt_token"];

        if (string.IsNullOrEmpty(jwtToken))
        {
            return BadRequest("JWT Token is not existing in header of the request");
        }

        try
        {
            // Dekodowanie tokena JWT
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            // Przygotowanie słownika na dane z payloadu
            var payloadDictionary = jsonToken.Payload.ToDictionary(x => x.Key, x => x.Value.ToString());

            // Wydobycie adresu email użytkownika
            string userEmail;
            if (payloadDictionary.TryGetValue("Email", out userEmail))
            {
                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest("Adres email użytkownika w tokenie JWT jest pusty.");
                }

                // Pobranie użytkownika na podstawie adresu email z bazy danych
                using (var session = NHibernateHelper.OpenSession())
                {
                    var user = session.Query<UserEntity>().FirstOrDefault(u => u.Email == userEmail);
                    if (user == null)
                    {
                        return NotFound("Użytkownik nie został znaleziony.");
                    }

                    return Ok(user);
                }
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Wystąpił błąd podczas weryfikacji tokenu JWT: " + ex.Message);
        }
    }

    
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken()
    {
        // Wydobycie tokenId (jti) z nagłówka cookie jwt_token
        var jwtToken = Request.Cookies["jwt_token"];

        // Sprawdzenie czy jwt_token został przekazany w nagłówku
        if (string.IsNullOrEmpty(jwtToken))
        {
            return BadRequest("Token JWT nie został przekazany w nagłówku cookie.");
        }

        try
        {
            // Dekodowanie tokena JWT
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            // Wydobycie jti (JWT ID) z tokenu JWT
            var jti = jsonToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                return BadRequest("Nie można wydobyć JWT ID (jti) z tokenu JWT.");
            }

            // Unieważnianie tokenu na podstawie jti w serwisie refresh tokenów
            await _refreshTokenService.RevokeRefreshToken(jti);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Wystąpił błąd podczas unieważniania tokenu JWT: " + ex.Message);
        }
    }
}