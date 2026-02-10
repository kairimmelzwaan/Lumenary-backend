using backend.Validation;

namespace backend.Dtos;

public sealed record ChangeEmailVerifyRequest(
    [param: NotEmptyGuid] Guid ChallengeId,
    [param: VerificationCode] string Code);
