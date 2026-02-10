using backend.Auth.Challenges;
using backend.Models;
using backend.Services.Results;

namespace backend.Services.Auth.ResendPolicies;

public sealed class PasswordResetResendPolicy : IChallengeResendPolicy
{
    public ChallengePurpose Purpose => ChallengePurpose.PasswordReset;

    public Result Validate(UserAuthChallenge challenge, Guid? requesterUserId)
    {
        if (string.IsNullOrWhiteSpace(challenge.TargetPhoneE164) ||
            !string.Equals(challenge.TargetPhoneE164, challenge.User.PhoneE164, StringComparison.Ordinal))
        {
            return Result.BadRequest();
        }

        return Result.Ok();
    }
}
