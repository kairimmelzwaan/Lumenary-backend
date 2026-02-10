using backend.Validation;

namespace backend.Dtos;

public sealed record ChangePhoneCancelRequest(
    [param: NotEmptyGuid] Guid ChallengeId);
