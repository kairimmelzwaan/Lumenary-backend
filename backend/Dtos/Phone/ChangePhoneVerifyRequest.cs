using backend.Validation;

namespace backend.Dtos;

public sealed record ChangePhoneVerifyRequest(
    [param: NotEmptyGuid] Guid ChallengeId,
    [param: VerificationCode] string Code);
