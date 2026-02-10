using backend.Dtos;
using backend.Services.Results;

namespace backend.Services.Appointments;

public interface IAppointmentsService
{
    Task<Result<AppointmentResponse>> CreateAsync(AppointmentCreateRequest request, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<AppointmentResponse>>> GetListAsync(
        AppointmentListQuery query,
        CancellationToken cancellationToken);
    Task<Result<AppointmentResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<AppointmentResponse>> UpdateAsync(
        Guid id,
        AppointmentUpdateRequest request,
        CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<AppointmentResponse>>> GetMineAsync(
        AppointmentListQuery query,
        CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<AppointmentResponse>>> GetForClientAsync(
        Guid clientId,
        AppointmentListQuery query,
        CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<AppointmentResponse>>> GetForTherapistAsync(
        Guid therapistUserId,
        AppointmentListQuery query,
        CancellationToken cancellationToken);
}
