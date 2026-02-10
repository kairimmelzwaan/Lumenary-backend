using backend.Validation;

namespace backend.Dtos;

public sealed record ChangeEmailCancelRequest(
    [param: NotEmptyGuid] Guid ChallengeId);
