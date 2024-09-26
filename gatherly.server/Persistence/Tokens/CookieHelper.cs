namespace gatherly.server.Persistence.Tokens;

public static class CookieHelper
{
    public static void SetJwtCookie(HttpResponse response, string token)
    {
        var jwtCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        response.Cookies.Append("Authorization", "Bearer " + token, jwtCookieOptions);
    }

    public static void SetRefreshTokenCookie(HttpResponse response, string refreshToken)
    {
        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        response.Cookies.Append("RefreshToken", refreshToken, refreshCookieOptions);
    }
    
    public static void ClearCookies(HttpResponse response)
    {
        response.Cookies.Delete("Authorization");
        response.Cookies.Delete("RefreshToken");
    }
}