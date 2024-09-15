using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.IdentityModel.Tokens;

namespace gatherly.server.Persistence.Tokens.TokenEntity;

/// <summary>
///     Repository for managing token entities.
/// </summary>
public class TokenEntityRepository : ITokenEntityRepository
{
    private readonly string _secret = Environment.GetEnvironmentVariable("SECRET");

    /// <summary>
    ///     Generates a JWT token for a given user.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="jti">The unique identifier for the token.</param>
    /// <returns>A JWT token string.</returns>
    public string GenerateToken(UserEntity user, string jti)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.UserRole.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            Env.GetString("ISSUER"),
            Env.GetString("AUDIENCE"),
            claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    ///     Validates a JWT token and returns the claims principal if valid.
    /// </summary>
    /// <param name="token">The JWT token string.</param>
    /// <returns>The claims principal if the token is valid; otherwise, null.</returns>
    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Env.GetString("ISSUER"),
                ValidAudience = Env.GetString("AUDIENCE"),
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var jti = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     Extracts the email from the JWT token present in the cookies.
    /// </summary>
    /// <param name="httpContext">The HTTP context containing the request cookies.</param>
    /// <returns>The email extracted from the token if valid; otherwise, null.</returns>
    public string? GetEmailFromRequestCookie(HttpContext httpContext)
    {
            string? token = httpContext.Request.Cookies["Authorization"];
            if (token == null) return null;

            if (!token.StartsWith("Bearer ")) return null;

            token = token.Substring("Bearer ".Length).Trim();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userMail = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub");

            return userMail?.Value;
    }
    
    /// <summary>
    ///     Extracts the email from the JWT token present in the cookies and retrieves the user ID.
    /// </summary>
    /// <param name="httpContext">The HTTP context containing the request cookies.</param>
    /// <returns>ID of the user if valid; otherwise, null.</returns>
    public string? GetIdFromRequestCookie(HttpContext httpContext)
    {
        string? token = httpContext.Request.Cookies["Authorization"];
        if (token == null) return null;

        if (!token.StartsWith("Bearer ")) return null;

        token = token.Substring("Bearer ".Length).Trim();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userEmail = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub").Value;
        
        using (var session = NHibernateHelper.OpenSession())
        {
            var user = session.Query<UserEntity>().FirstOrDefault(x => x.Email == userEmail);
            return user?.Id.ToString();
        }
    }

}