using System.ComponentModel.DataAnnotations;

namespace backend.Dtos;

public sealed record AppointmentUpdateRequest(
    DateTime? StartsAt,
    DateTime? EndsAt,
    [param: StringLength(64)] string? Timezone,
    [param: StringLength(32)] string? Status,
    [param: StringLength(32)] string? Mode,
    [param: StringLength(256)] string? Location,
    string? Notes,
    DateTime? CanceledAt,
    [param: StringLength(256)] string? CancellationReason);
