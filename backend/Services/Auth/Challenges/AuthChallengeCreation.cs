namespace backend.Services.Auth;

public sealed record AuthChallengeCreation(Guid ChallengeId, string Code);
