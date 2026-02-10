using System.ComponentModel.DataAnnotations;
using backend.Validation;

namespace backend.Dtos;

public sealed record AppointmentCreateRequest(
    Guid ClientId,
    Guid TherapistUserId,
    DateTime StartsAt,
    DateTime EndsAt,
    [param: NotWhiteSpace, StringLength(64)] string Timezone,
    [param: NotWhiteSpace, StringLength(32)] string Mode,
    [param: StringLength(32)] string? Status,
    [param: StringLength(256)] string? Location,
    string? Notes);
