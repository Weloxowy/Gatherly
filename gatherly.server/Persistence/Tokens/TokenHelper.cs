using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;

namespace gatherly.server.Persistence.Tokens;

public class TokenHelper
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ITokenEntityService _tokenEntityService;

    public TokenHelper(IRefreshTokenService refreshTokenService, ITokenEntityService tokenEntityService)
    {
        _refreshTokenService = refreshTokenService;
        _tokenEntityService = tokenEntityService;
    }

    public TokenHelper() : base()
    {
       
    }
    public virtual (string JwtToken, string RefreshToken) GenerateTokens(UserEntity user)
    {
        var refresh = _refreshTokenService.GenerateRefreshToken(user.Id);
        var jwt = _tokenEntityService.GenerateToken(user, refresh.Id.ToString());
        return (jwt, refresh.Token);
    }
}