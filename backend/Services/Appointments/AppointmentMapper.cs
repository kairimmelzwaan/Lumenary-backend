using System.Linq.Expressions;
using backend.Dtos;
using backend.Models;

namespace backend.Services.Appointments;

public static class AppointmentMapper
{
    public static readonly Expression<Func<Appointment, AppointmentResponse>> Projection = appointment
        => new AppointmentResponse(
            appointment.Id,
            appointment.ClientId,
            appointment.TherapistUserId,
            appointment.CreatedByUserId,
            appointment.StartsAt,
            appointment.EndsAt,
            appointment.Timezone,
            appointment.Status,
            appointment.Mode,
            appointment.Location,
            appointment.Notes,
            appointment.CanceledAt,
            appointment.CancellationReason,
            appointment.CreatedAt,
            appointment.UpdatedAt);

    public static AppointmentResponse FromEntity(Appointment appointment)
        => new(
            appointment.Id,
            appointment.ClientId,
            appointment.TherapistUserId,
            appointment.CreatedByUserId,
            appointment.StartsAt,
            appointment.EndsAt,
            appointment.Timezone,
            appointment.Status,
            appointment.Mode,
            appointment.Location,
            appointment.Notes,
            appointment.CanceledAt,
            appointment.CancellationReason,
            appointment.CreatedAt,
            appointment.UpdatedAt);
}
