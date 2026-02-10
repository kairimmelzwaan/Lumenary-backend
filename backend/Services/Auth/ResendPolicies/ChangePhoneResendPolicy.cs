using backend.Auth.Challenges;
using backend.Models;
using backend.Services.Results;

namespace backend.Services.Auth.ResendPolicies;

public sealed class ChangePhoneResendPolicy : IChallengeResendPolicy
{
    public ChallengePurpose Purpose => ChallengePurpose.ChangePhone;

    public Result Validate(UserAuthChallenge challenge, Guid? requesterUserId)
    {
        if (!requesterUserId.HasValue || challenge.UserId != requesterUserId.Value)
        {
            return Result.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(challenge.User.PendingPhoneE164) ||
            !string.Equals(challenge.TargetPhoneE164, challenge.User.PendingPhoneE164, StringComparison.Ordinal))
        {
            return Result.BadRequest();
        }

        return Result.Ok();
    }
}
