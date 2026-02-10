using backend.Dtos;
using backend.Services.Auth;
using backend.Services.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace backend.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        => Exec(() => authService.LoginAsync(request, cancellationToken));

    [HttpPost("login/verify")]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> LoginVerify([FromBody] LoginVerifyRequest request,
        CancellationToken cancellationToken)
        => Exec(() => authService.LoginVerifyAsync(request, cancellationToken));

    [HttpPost("register")]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        => Exec(() => authService.RegisterAsync(request, cancellationToken));

    [HttpPost("register/verify")]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> RegisterVerify([FromBody] RegisterVerifyRequest request,
        CancellationToken cancellationToken)
        => Exec(() => authService.RegisterVerifyAsync(request, cancellationToken));

    [HttpPost("password/reset")]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> PasswordReset([FromBody] PasswordResetRequest request,
        CancellationToken cancellationToken)
        => Exec(() => authService.PasswordResetAsync(request, cancellationToken));

    [HttpPost("password/reset/verify")]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> PasswordResetVerify([FromBody] PasswordResetVerifyRequest request,
        CancellationToken cancellationToken)
        => Exec(() => authService.PasswordResetVerifyAsync(request, cancellationToken));

    [HttpPost("challenge/resend")]
    [EnableRateLimiting("Auth")]
    public Task<IActionResult> ResendChallengeCode([FromBody] ResendCodeRequest request,
        CancellationToken cancellationToken)
        => Exec(() => authService.ResendChallengeCodeAsync(request, cancellationToken));

    private static async Task<IActionResult> Exec(Func<Task<Result>> action)
        => (await action()).ToActionResult();

    private static async Task<IActionResult> Exec<T>(Func<Task<Result<T>>> action)
        => (await action()).ToActionResult();
}
