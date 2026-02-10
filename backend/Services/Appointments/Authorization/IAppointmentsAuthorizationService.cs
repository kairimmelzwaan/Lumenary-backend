using backend.Dtos;
using backend.Models;
using backend.Services.Appointments;
using backend.Services.Results;

namespace backend.Services.Appointments.Authorization;

public interface IAppointmentsAuthorizationService
{
    Task<Result> CanCreateAsync(
        AppointmentUserContext currentUser,
        AppointmentCreateRequest request,
        Client client,
        CancellationToken cancellationToken);
    Task<Result> CanAccessAppointmentAsync(
        AppointmentUserContext currentUser,
        Appointment appointment,
        CancellationToken cancellationToken);
    Task<Result<AppointmentListScope>> ApplyListScopeAsync(
        AppointmentUserContext currentUser,
        AppointmentListScopeRequest request,
        CancellationToken cancellationToken);
    Task<Result> CanAccessClientAppointmentsAsync(
        AppointmentUserContext currentUser,
        Guid clientId,
        CancellationToken cancellationToken);
    Result CanAccessTherapistAppointments(AppointmentUserContext currentUser, Guid therapistUserId);
}
