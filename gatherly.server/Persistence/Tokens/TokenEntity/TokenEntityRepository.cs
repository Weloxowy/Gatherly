using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using gatherly.server.Models.Tokens.TokenEntity;
using Microsoft.IdentityModel.Tokens;

namespace gatherly.server.Persistence.Tokens.TokenEntity;

public class TokenEntityRepository : ITokenEntityRepository
{
    private readonly string _secret = Environment.GetEnvironmentVariable("SECRET");
    
    public string GenerateToken(Models.Authentication.UserEntity.UserEntity user, string jti)
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
            issuer: "localhost:44329",
            audience: "localhost:3000",
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

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
                ValidIssuer = "localhost:44329",
                ValidAudience = "localhost:3000",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var jti = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            
            return principal;
        }
        catch
        {
            return null;
        }
    }
    
    public string GetEmailFromRequestHeader(HttpContext httpContext)
    {
        // Pobranie tokena JWT z nagłówka żądania
        var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
        {
            return null;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        // Zdekodowanie tokena JWT w celu uzyskania identyfikatora użytkownika
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userMail = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub"); // "sub" jest często używanym typem claimu dla identyfikatora użytkownika

        if (userMail == null)
        {
            return null;
        }

        return userMail.Value.ToString();
    }
}