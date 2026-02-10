namespace backend.Services.Auth;

public sealed record AuthChallengeResendResult(Guid ChallengeId, string? Code);
