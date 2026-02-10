using backend.Dtos;
using backend.Services.Appointments;
using backend.Services.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Authorize]
[Route("api/appointments")]
public class AppointmentsController(IAppointmentsService appointmentsService) : ControllerBase
{
    [HttpPost]
    public Task<IActionResult> Create(
        [FromBody] AppointmentCreateRequest request,
        CancellationToken cancellationToken)
        => Exec(
            () => appointmentsService.CreateAsync(request, cancellationToken),
            response => CreatedAtAction(nameof(GetById), new { id = response.Id }, response));

    [HttpGet]
    public Task<IActionResult> GetList(
        [FromQuery] Guid? clientId,
        [FromQuery] Guid? therapistUserId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
        => Exec(() => appointmentsService.GetListAsync(
            new AppointmentListQuery(clientId, therapistUserId, from, to, status),
            cancellationToken));

    [HttpGet("{id:guid}")]
    public Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Exec(() => appointmentsService.GetByIdAsync(id, cancellationToken));

    [HttpPatch("{id:guid}")]
    public Task<IActionResult> Update(
        Guid id,
        [FromBody] AppointmentUpdateRequest request,
        CancellationToken cancellationToken)
        => Exec(() => appointmentsService.UpdateAsync(id, request, cancellationToken));

    [HttpGet("/api/me/appointments")]
    public Task<IActionResult> GetMine(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
        => Exec(() => appointmentsService.GetMineAsync(
            new AppointmentListQuery(null, null, from, to, status),
            cancellationToken));

    [HttpGet("/api/clients/{clientId:guid}/appointments")]
    public Task<IActionResult> GetForClient(
        Guid clientId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
        => Exec(() => appointmentsService.GetForClientAsync(
            clientId,
            new AppointmentListQuery(clientId, null, from, to, status),
            cancellationToken));

    [HttpGet("/api/therapists/{therapistUserId:guid}/appointments")]
    public Task<IActionResult> GetForTherapist(
        Guid therapistUserId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
        => Exec(() => appointmentsService.GetForTherapistAsync(
            therapistUserId,
            new AppointmentListQuery(null, therapistUserId, from, to, status),
            cancellationToken));

    private async Task<IActionResult> Exec(Func<Task<Result>> action)
        => (await action()).ToActionResult();

    private async Task<IActionResult> Exec<T>(Func<Task<Result<T>>> action)
        => (await action()).ToActionResult();

    private async Task<IActionResult> Exec<T>(Func<Task<Result<T>>> action, Func<T, IActionResult> onSuccess)
    {
        var result = await action();
        if (result.IsSuccess && result.Value is not null)
            return onSuccess(result.Value);

        return result.ToActionResult();
    }
}