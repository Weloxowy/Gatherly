using System.Security.Claims;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.TokenEntity;

namespace gatherly.server.Persistence.Tokens.TokenEntity;

public class TokenEntityService : ITokenEntityService
{
    private readonly TokenEntityRepository _tokenEntityRepository = new();

    public string GenerateToken(UserEntity user, string jti)
    {
        return _tokenEntityRepository.GenerateToken(user, jti);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        return _tokenEntityRepository.ValidateToken(token);
    }

   
    public string? GetEmailFromRequestCookie(HttpContext httpContext)
    {
        return _tokenEntityRepository.GetEmailFromRequestCookie(httpContext);
    }

    public string? GetIdFromRequestCookie(HttpContext httpContext)
    {
        return _tokenEntityRepository.GetIdFromRequestCookie(httpContext);
    }
    
}