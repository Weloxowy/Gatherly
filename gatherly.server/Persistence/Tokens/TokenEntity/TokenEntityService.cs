using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using gatherly.server.Models.Tokens.TokenEntity;

namespace gatherly.server.Persistence.Tokens.TokenEntity;

public class TokenEntityService : ITokenEntityService
{
    private TokenEntityRepository _tokenEntityRepository = new TokenEntityRepository();

    public string GenerateToken(Models.Authentication.UserEntity.UserEntity user, string jti)
    {
        return _tokenEntityRepository.GenerateToken(user, jti);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        return _tokenEntityRepository.ValidateToken(token);
    }
    
    public string GetEmailFromRequestHeader(HttpContext httpContext)
    {
        return _tokenEntityRepository.GetEmailFromRequestHeader(httpContext);
    }
    
}