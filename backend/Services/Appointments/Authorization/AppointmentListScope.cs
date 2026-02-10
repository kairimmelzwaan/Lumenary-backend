namespace backend.Services.Appointments.Authorization;

public readonly record struct AppointmentListScope(Guid? ClientId, Guid? TherapistUserId);

public readonly record struct AppointmentListScopeRequest(Guid? ClientId, Guid? TherapistUserId);
