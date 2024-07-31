using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.RefreshToken;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.AspNetCore.Mvc;

namespace gatherly.server.Controllers.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ITokenEntityService _tokenEntityService;
    private readonly IUserEntityService _userService;
    
    /// <summary>
    ///     Constructor for UserController.
    /// </summary>
    /// <param name="userService">Service for user-related operations.</param>
    /// <param name="tokenEntityService">Service for token-related operations.</param>
    /// <param name="refreshTokenService">Service for refresh token operations.</param>
    public AuthorizationController(IUserEntityService userService, ITokenEntityService tokenEntityService,
        IRefreshTokenService refreshTokenService)
    {
        _userService = userService;
        _tokenEntityService = tokenEntityService;
        _refreshTokenService = refreshTokenService;
    }
    
    //czy Uzytkownik jest userem = user=0
    //czy uzytkownik ma prawa admina; user=1
    //podniesienie uprawnien
    //obnizenie uprawnien
    //danie uprawnien admina spotkania
    //odebranie uprawnien admina spotkania
    
}