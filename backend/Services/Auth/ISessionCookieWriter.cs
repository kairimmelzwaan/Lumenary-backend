namespace backend.Services.Auth;

public interface ISessionCookieWriter
{
    void WriteSessionCookie(string token, DateTime expiresAt);
    void ClearSessionCookie();
}
