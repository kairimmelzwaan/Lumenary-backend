using backend.Auth.Options;
using Microsoft.Extensions.Options;

namespace backend.Services.Auth;

public sealed class HttpSessionCookieWriter : ISessionCookieWriter
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthOptions _options;

    public HttpSessionCookieWriter(IHttpContextAccessor httpContextAccessor, IOptions<AuthOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public void WriteSessionCookie(string token, DateTime expiresAt)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is required to set session cookies.");

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt,
            IsEssential = true
        };

        // Ensure ForwardedHeaders middleware is configured when behind a proxy.
        httpContext.Response.Cookies.Append(_options.CookieName, token, cookieOptions);
    }

    public void ClearSessionCookie()
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is required to clear session cookies.");

        httpContext.Response.Cookies.Delete(_options.CookieName);
    }
}
