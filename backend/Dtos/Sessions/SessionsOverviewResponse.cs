namespace backend.Dtos;

public sealed record SessionsOverviewResponse(
    DateTime? LastLoginAt,
    string? LastLoginUserAgent,
    string? LastLoginIpAddress,
    IReadOnlyList<SessionResponse> Sessions);
