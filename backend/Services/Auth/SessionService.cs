using backend.Auth.Options;
using backend.Auth.Sessions;
using backend.Data;
using backend.Models;
using backend.Services.Requests;
using Microsoft.Extensions.Options;

namespace backend.Services.Auth;

public sealed class SessionService : ISessionService
{
    private readonly AppDbContext _dbContext;
    private readonly AuthOptions _options;
    private readonly IRequestMetadataAccessor _requestMetadataAccessor;
    private readonly ISessionCookieWriter _sessionCookieWriter;

    public SessionService(
        AppDbContext dbContext,
        IOptions<AuthOptions> options,
        IRequestMetadataAccessor requestMetadataAccessor,
        ISessionCookieWriter sessionCookieWriter)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _requestMetadataAccessor = requestMetadataAccessor;
        _sessionCookieWriter = sessionCookieWriter;
    }

    public async Task CreateSessionAndSetCookieAsync(User user, DateTime now, CancellationToken cancellationToken)
    {
        var token = SessionTokenUtilities.CreateToken(_options.SessionTokenKey, out var tokenHash);
        var expiresAt = now.AddDays(_options.SessionTtlDays);

        var session = new Session
        {
            UserId = user.Id,
            SessionTokenHash = tokenHash,
            UserAgent = _requestMetadataAccessor.UserAgent,
            IpAddress = _requestMetadataAccessor.IpAddress,
            CreatedAt = now,
            LastSeenAt = now,
            ExpiresAt = expiresAt
        };

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _sessionCookieWriter.WriteSessionCookie(token, expiresAt);
    }

    public void ClearSessionCookie()
    {
        _sessionCookieWriter.ClearSessionCookie();
    }
}
