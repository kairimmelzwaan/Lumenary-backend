using backend.Validation;

namespace backend.Dtos;

public sealed record RegisterVerifyRequest(
    [param: NotEmptyGuid] Guid ChallengeId,
    [param: VerificationCode] string Code);
