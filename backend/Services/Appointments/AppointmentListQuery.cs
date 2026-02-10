namespace backend.Services.Appointments;

public sealed record AppointmentListQuery(
    Guid? ClientId,
    Guid? TherapistUserId,
    DateTime? From,
    DateTime? To,
    string? Status);
