using System.Security.Claims;
using gatherly.server.Models.Authentication.UserEntity;

namespace gatherly.server.Models.Tokens.TokenEntity;

public interface ITokenEntityService
{
    public string GenerateToken(UserEntity userEntity, string jti); //tworzenie tokena jwt
    public ClaimsPrincipal ValidateToken(string token); //sprawdzanie czy token jest poprawny
    public string? GetEmailFromRequestCookie(HttpContext httpContext);
    public string? GetIdFromRequestCookie(HttpContext httpContext);
}