using backend.Auth.Challenges;
using backend.Models;
using backend.Services.Results;

namespace backend.Services.Auth.ResendPolicies;

public sealed class RegisterResendPolicy : IChallengeResendPolicy
{
    public ChallengePurpose Purpose => ChallengePurpose.Register;

    public Result Validate(UserAuthChallenge challenge, Guid? requesterUserId)
    {
        return Result.Ok();
    }
}
