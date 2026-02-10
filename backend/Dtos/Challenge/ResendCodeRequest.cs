using backend.Validation;

namespace backend.Dtos;

public sealed record ResendCodeRequest(
    [param: NotEmptyGuid] Guid ChallengeId);
