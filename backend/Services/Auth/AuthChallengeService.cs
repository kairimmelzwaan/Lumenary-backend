using backend.Auth.Challenges;
using backend.Auth.Options;
using backend.Auth.Verification;
using backend.Data;
using backend.Models;
using backend.Services.Auth.ResendPolicies;
using backend.Services.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backend.Services.Auth;

public sealed class AuthChallengeService : IAuthChallengeService
{
    private readonly AppDbContext _dbContext;
    private readonly AuthOptions _options;
    private readonly IReadOnlyDictionary<ChallengePurpose, IChallengeResendPolicy> _resendPolicies;

    public AuthChallengeService(
        AppDbContext dbContext,
        IOptions<AuthOptions> options,
        IEnumerable<IChallengeResendPolicy> resendPolicies)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _resendPolicies = resendPolicies.ToDictionary(policy => policy.Purpose);
    }

    public async Task<AuthChallengeCreation> CreateChallengeAsync(
        AuthChallengeRequest request,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var code = VerificationCodeUtilities.CreateCode();
        var codeHash = VerificationCodeUtilities.ComputeHash(code, _options.SessionTokenKey);
        var expiresAt = now.AddMinutes(_options.LoginCodeTtlMinutes);

        var challenge = new UserAuthChallenge
        {
            UserId = request.UserId,
            Purpose = request.Purpose.ToValue(),
            TargetEmail = request.TargetEmail,
            TargetPhoneE164 = request.TargetPhoneE164,
            CodeHash = codeHash,
            AttemptCount = 0,
            CreatedAt = now,
            ExpiresAt = expiresAt
        };

        if (request.ChallengeId.HasValue)
        {
            challenge.Id = request.ChallengeId.Value;
        }

        _dbContext.UserAuthChallenges.Add(challenge);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthChallengeCreation(challenge.Id, code);
    }

    public async Task<Result<UserAuthChallenge>> GetActiveChallengeAsync(
        Guid challengeId,
        ChallengePurpose purpose,
        Guid? userId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.UserAuthChallenges
            .Include(c => c.User)
            .Where(c => c.Id == challengeId &&
                        c.Purpose == purpose.ToValue() &&
                        c.VerifiedAt == null &&
                        c.ExpiresAt > now);

        if (userId.HasValue)
        {
            query = query.Where(c => c.UserId == userId.Value);
        }

        var challenge = await query.FirstOrDefaultAsync(cancellationToken);
        if (challenge == null || !challenge.User.IsActive)
        {
            return Result<UserAuthChallenge>.Unauthorized();
        }

        return Result<UserAuthChallenge>.Ok(challenge);
    }

    public async Task<Result> VerifyChallengeCodeAsync(
        UserAuthChallenge challenge,
        string code,
        DateTime now,
        CancellationToken cancellationToken)
    {
        if (challenge.AttemptCount >= AuthChallengePolicy.MaxAttempts)
        {
            return Result.TooManyRequests();
        }

        var providedHash = VerificationCodeUtilities.ComputeHash(code, _options.SessionTokenKey);
        if (!challenge.CodeHash.SequenceEqual(providedHash))
        {
            if (challenge.AttemptCount < AuthChallengePolicy.MaxAttempts)
            {
                challenge.AttemptCount += 1;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Unauthorized();
        }

        challenge.VerifiedAt = now;
        return Result.Ok();
    }

    public async Task<Result<AuthChallengeResendResult>> ResendChallengeAsync(
        Guid challengeId,
        Guid? requesterUserId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var challenge = await _dbContext.UserAuthChallenges
            .Include(c => c.User)
            .Where(c => c.Id == challengeId &&
                        c.VerifiedAt == null &&
                        c.ExpiresAt > now)
            .FirstOrDefaultAsync(cancellationToken);

        if (challenge == null || !challenge.User.IsActive)
        {
            return Result<AuthChallengeResendResult>.NotFound();
        }

        if (!ChallengePurposeExtensions.TryParse(challenge.Purpose, out var purpose))
        {
            return Result<AuthChallengeResendResult>.BadRequest();
        }

        if (!_resendPolicies.TryGetValue(purpose, out var policy))
        {
            return Result<AuthChallengeResendResult>.BadRequest();
        }

        var policyResult = policy.Validate(challenge, requesterUserId);
        if (!policyResult.IsSuccess)
        {
            return policyResult.Status switch
            {
                ResultStatus.Unauthorized => Result<AuthChallengeResendResult>.Unauthorized(),
                ResultStatus.BadRequest => Result<AuthChallengeResendResult>.BadRequest(),
                _ => Result<AuthChallengeResendResult>.BadRequest()
            };
        }

        var code = VerificationCodeUtilities.CreateCode();
        challenge.CodeHash = VerificationCodeUtilities.ComputeHash(code, _options.SessionTokenKey);
        challenge.AttemptCount = 0;
        challenge.ExpiresAt = now.AddMinutes(_options.LoginCodeTtlMinutes);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var responseCode = purpose is ChallengePurpose.Login or ChallengePurpose.Register ? code : null;
        return Result<AuthChallengeResendResult>.Ok(new AuthChallengeResendResult(challenge.Id, responseCode));
    }
}
