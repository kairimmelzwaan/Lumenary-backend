using backend.Dtos;
using backend.Services.Account;
using backend.Services.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace backend.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public Task<IActionResult> GetMe(CancellationToken cancellationToken)
        => Exec(() => accountService.GetMeAsync(cancellationToken));

    [HttpPatch]
    [Authorize]
    public Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
        => Exec(() => accountService.UpdateProfileAsync(request, cancellationToken));

    [HttpPost("password/change")]
    [Authorize]
    public Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest request,
        CancellationToken cancellationToken)
        => Exec(() => accountService.ChangePasswordAsync(request, cancellationToken));

    [HttpPost("email/change")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request,
        CancellationToken cancellationToken)
        => Exec(() => accountService.ChangeEmailAsync(request, cancellationToken));

    [HttpPost("email/change/verify")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> ChangeEmailVerify(
        [FromBody] ChangeEmailVerifyRequest request,
        CancellationToken cancellationToken)
        => Exec(() => accountService.ChangeEmailVerifyAsync(request, cancellationToken));

    [HttpPost("email/change/cancel")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> ChangeEmailCancel(
        [FromBody] ChangeEmailCancelRequest request,
        CancellationToken cancellationToken)
        => Exec(() => accountService.ChangeEmailCancelAsync(request, cancellationToken));

    [HttpPost("phone/change")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> ChangePhone([FromBody] ChangePhoneRequest request,
        CancellationToken cancellationToken)
        => Exec(() => accountService.ChangePhoneAsync(request, cancellationToken));

    [HttpPost("phone/change/verify")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> ChangePhoneVerify(
        [FromBody] ChangePhoneVerifyRequest request,
        CancellationToken cancellationToken)
        => Exec(() => accountService.ChangePhoneVerifyAsync(request, cancellationToken));

    [HttpPost("phone/change/cancel")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> ChangePhoneCancel(
        [FromBody] ChangePhoneCancelRequest request,
        CancellationToken cancellationToken)
        => Exec(() => accountService.ChangePhoneCancelAsync(request, cancellationToken));

    [HttpGet("sessions")]
    [Authorize]
    public Task<IActionResult> GetSessions(CancellationToken cancellationToken)
        => Exec(() => accountService.GetSessionsAsync(cancellationToken));

    [HttpPost("logout")]
    [Authorize]
    public Task<IActionResult> Logout(CancellationToken cancellationToken)
        => Exec(() => accountService.LogoutAsync(cancellationToken));

    [HttpPost("logout/all")]
    [Authorize]
    public Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
        => Exec(() => accountService.LogoutAllAsync(cancellationToken));

    private static async Task<IActionResult> Exec(Func<Task<Result>> action)
        => (await action()).ToActionResult();

    private static async Task<IActionResult> Exec<T>(Func<Task<Result<T>>> action)
        => (await action()).ToActionResult();
}
