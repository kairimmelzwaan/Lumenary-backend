using backend.Auth.Challenges;
using backend.Models;
using backend.Services.Results;

namespace backend.Services.Auth.ResendPolicies;

public interface IChallengeResendPolicy
{
    ChallengePurpose Purpose { get; }
    Result Validate(UserAuthChallenge challenge, Guid? requesterUserId);
}
