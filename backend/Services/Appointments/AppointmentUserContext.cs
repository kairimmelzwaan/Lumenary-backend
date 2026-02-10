namespace backend.Services.Appointments;

public readonly record struct AppointmentUserContext(Guid UserId, string? Role);
