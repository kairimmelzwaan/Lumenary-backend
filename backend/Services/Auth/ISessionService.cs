using backend.Models;

namespace backend.Services.Auth;

public interface ISessionService
{
    Task CreateSessionAndSetCookieAsync(User user, DateTime now, CancellationToken cancellationToken);
    void ClearSessionCookie();
}
