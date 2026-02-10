using backend.Validation;

namespace backend.Dtos;

public sealed record LoginVerifyRequest(
    [param: NotEmptyGuid] Guid ChallengeId,
    [param: VerificationCode] string Code);
