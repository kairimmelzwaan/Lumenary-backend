using backend.Auth.Challenges;
using backend.Models;
using backend.Services.Results;

namespace backend.Services.Auth;

public interface IAuthChallengeService
{
    Task<AuthChallengeCreation> CreateChallengeAsync(
        AuthChallengeRequest request,
        DateTime now,
        CancellationToken cancellationToken);

    Task<Result<UserAuthChallenge>> GetActiveChallengeAsync(
        Guid challengeId,
        ChallengePurpose purpose,
        Guid? userId,
        DateTime now,
        CancellationToken cancellationToken);

    Task<Result> VerifyChallengeCodeAsync(
        UserAuthChallenge challenge,
        string code,
        DateTime now,
        CancellationToken cancellationToken);

    Task<Result<AuthChallengeResendResult>> ResendChallengeAsync(
        Guid challengeId,
        Guid? requesterUserId,
        DateTime now,
        CancellationToken cancellationToken);
}
