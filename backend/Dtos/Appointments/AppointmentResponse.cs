namespace backend.Dtos;

public sealed record AppointmentResponse(
    Guid Id,
    Guid ClientId,
    Guid TherapistUserId,
    Guid CreatedByUserId,
    DateTime StartsAt,
    DateTime EndsAt,
    string Timezone,
    string Status,
    string Mode,
    string? Location,
    string? Notes,
    DateTime? CanceledAt,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime UpdatedAt);
