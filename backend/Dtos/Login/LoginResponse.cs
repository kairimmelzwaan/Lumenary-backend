namespace backend.Dtos;

public sealed record LoginResponse(Guid ChallengeId, string Code);
