using backend.Auth.Challenges;

namespace backend.Services.Auth;

public sealed record AuthChallengeRequest(
    Guid UserId,
    ChallengePurpose Purpose,
    string? TargetEmail,
    string? TargetPhoneE164,
    Guid? ChallengeId = null);
