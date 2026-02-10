namespace backend.Dtos;

public sealed record SessionResponse(
    DateTime CreatedAt,
    DateTime LastSeenAt,
    DateTime ExpiresAt,
    string? UserAgent,
    string? IpAddress);
