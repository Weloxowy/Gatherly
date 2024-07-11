using System.Security.Claims;

namespace gatherly.server.Models.Tokens.TokenEntity;

public interface ITokenEntityRepository
{
    public string GenerateToken(Models.Authentication.UserEntity.UserEntity userEntity, string jti); //tworzenie tokena jwt
    public ClaimsPrincipal ValidateToken(string token); //sprawdzanie czy token jest poprawny
    
    public string GetEmailFromRequestHeader(HttpContext httpContext);
}