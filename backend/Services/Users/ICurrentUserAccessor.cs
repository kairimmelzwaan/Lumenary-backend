namespace backend.Services.Users;

public interface ICurrentUserAccessor
{
    bool TryGetUserId(out Guid userId);
    bool TryGetSessionId(out Guid sessionId);
    bool TryGetRole(out string? role);
}
