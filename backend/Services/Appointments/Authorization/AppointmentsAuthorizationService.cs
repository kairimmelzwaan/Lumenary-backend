using backend.Auth.Identity;
using backend.Data;
using backend.Dtos;
using backend.Models;
using backend.Services.Appointments;
using backend.Services.Results;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Appointments.Authorization;

public sealed class AppointmentsAuthorizationService(AppDbContext dbContext) : IAppointmentsAuthorizationService
{
    public async Task<Result> CanCreateAsync(
        AppointmentUserContext currentUser,
        AppointmentCreateRequest request,
        Client client,
        CancellationToken cancellationToken)
    {
        if (AppointmentRolePolicy.CanManageAll(currentUser.Role))
            return Result.Ok();

        if (string.Equals(currentUser.Role, UserRoles.Therapist, StringComparison.Ordinal))
        {
            if (request.TherapistUserId != currentUser.UserId || client.TherapistUserId != currentUser.UserId)
                return Result.Forbidden();

            return Result.Ok();
        }

        if (string.Equals(currentUser.Role, UserRoles.Client, StringComparison.Ordinal))
        {
            var ownClientId = await GetClientIdForUserAsync(currentUser.UserId, cancellationToken);
            if (!ownClientId.HasValue)
                return Result.Unauthorized();

            if (ownClientId.Value != request.ClientId)
                return Result.Forbidden();

            return Result.Ok();
        }

        return Result.Forbidden();
    }

    public async Task<Result> CanAccessAppointmentAsync(
        AppointmentUserContext currentUser,
        Appointment appointment,
        CancellationToken cancellationToken)
    {
        if (AppointmentRolePolicy.CanManageAll(currentUser.Role))
            return Result.Ok();

        if (string.Equals(currentUser.Role, UserRoles.Therapist, StringComparison.Ordinal))
            return appointment.TherapistUserId == currentUser.UserId ? Result.Ok() : Result.Forbidden();

        if (string.Equals(currentUser.Role, UserRoles.Client, StringComparison.Ordinal))
        {
            var clientId = await GetClientIdForUserAsync(currentUser.UserId, cancellationToken);
            if (clientId.HasValue && appointment.ClientId == clientId.Value)
                return Result.Ok();

            return Result.Forbidden();
        }

        return Result.Forbidden();
    }

    public async Task<Result<AppointmentListScope>> ApplyListScopeAsync(
        AppointmentUserContext currentUser,
        AppointmentListScopeRequest request,
        CancellationToken cancellationToken)
    {
        if (AppointmentRolePolicy.CanManageAll(currentUser.Role))
            return Result<AppointmentListScope>.Ok(new AppointmentListScope(request.ClientId, request.TherapistUserId));

        if (string.Equals(currentUser.Role, UserRoles.Client, StringComparison.Ordinal))
        {
            var ownClientId = await GetClientIdForUserAsync(currentUser.UserId, cancellationToken);
            if (!ownClientId.HasValue)
                return Result<AppointmentListScope>.Unauthorized();

            if (request.ClientId.HasValue && request.ClientId.Value != ownClientId.Value)
                return Result<AppointmentListScope>.Forbidden();

            return Result<AppointmentListScope>.Ok(new AppointmentListScope(ownClientId.Value, request.TherapistUserId));
        }

        if (string.Equals(currentUser.Role, UserRoles.Therapist, StringComparison.Ordinal))
        {
            if (request.TherapistUserId.HasValue && request.TherapistUserId.Value != currentUser.UserId)
                return Result<AppointmentListScope>.Forbidden();

            return Result<AppointmentListScope>.Ok(new AppointmentListScope(request.ClientId, currentUser.UserId));
        }

        return Result<AppointmentListScope>.Forbidden();
    }

    public async Task<Result> CanAccessClientAppointmentsAsync(
        AppointmentUserContext currentUser,
        Guid clientId,
        CancellationToken cancellationToken)
    {
        if (AppointmentRolePolicy.CanManageAll(currentUser.Role))
            return Result.Ok();

        if (string.Equals(currentUser.Role, UserRoles.Therapist, StringComparison.Ordinal))
        {
            var ownsClient = await dbContext.Clients
                .AsNoTracking()
                .AnyAsync(
                    client => client.Id == clientId && client.TherapistUserId == currentUser.UserId,
                    cancellationToken);

            return ownsClient ? Result.Ok() : Result.Forbidden();
        }

        if (string.Equals(currentUser.Role, UserRoles.Client, StringComparison.Ordinal))
        {
            var ownClientId = await GetClientIdForUserAsync(currentUser.UserId, cancellationToken);
            return ownClientId.HasValue && ownClientId.Value == clientId
                ? Result.Ok()
                : Result.Forbidden();
        }

        return Result.Forbidden();
    }

    public Result CanAccessTherapistAppointments(AppointmentUserContext currentUser, Guid therapistUserId)
    {
        if (AppointmentRolePolicy.CanManageAll(currentUser.Role))
            return Result.Ok();

        return therapistUserId == currentUser.UserId ? Result.Ok() : Result.Forbidden();
    }

    private async Task<Guid?> GetClientIdForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Clients
            .AsNoTracking()
            .Where(client => client.UserId == userId)
            .Select(client => (Guid?)client.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
