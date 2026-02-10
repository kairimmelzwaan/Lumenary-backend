using backend.Auth.Identity;
using backend.Data;
using backend.Domain.ValueObjects;
using backend.Dtos;
using backend.Models;
using backend.Services.Appointments.Authorization;
using backend.Services.Results;
using backend.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Appointments;

public sealed class AppointmentsService(
    AppDbContext dbContext,
    IAppointmentsAuthorizationService authorizationService,
    ICurrentUserAccessor currentUserAccessor)
    : IAppointmentsService
{
    public async Task<Result<AppointmentResponse>> CreateAsync(
        AppointmentCreateRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUser))
            return Result<AppointmentResponse>.Unauthorized();

        if (!AppointmentStatus.TryParse(request.Status, AppointmentStatusParseMode.DefaultToScheduled, out var status))
            return Result<AppointmentResponse>.BadRequest();

        if (!AppointmentMode.TryParse(request.Mode, out var mode))
            return Result<AppointmentResponse>.BadRequest();

        if (request.EndsAt <= request.StartsAt)
            return Result<AppointmentResponse>.BadRequest();

        var client = await dbContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);
        if (client == null)
            return Result<AppointmentResponse>.NotFound();

        var therapistExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.TherapistUserId &&
                           u.Role == UserRoles.Therapist &&
                           u.IsActive,
                cancellationToken);
        if (!therapistExists)
            return Result<AppointmentResponse>.BadRequest();

        if (client.TherapistUserId != request.TherapistUserId)
            return Result<AppointmentResponse>.BadRequest();

        var authResult = await authorizationService.CanCreateAsync(currentUser, request, client, cancellationToken);
        if (!authResult.IsSuccess)
            return MapFailure<AppointmentResponse>(authResult);

        var now = DateTime.UtcNow;
        var appointment = new Appointment
        {
            ClientId = request.ClientId,
            TherapistUserId = request.TherapistUserId,
            CreatedByUserId = currentUser.UserId,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            Timezone = request.Timezone.Trim(),
            Status = status.Value,
            Mode = mode.Value,
            Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim(),
            Notes = request.Notes,
            CreatedAt = now,
            UpdatedAt = now
        };

        if (status.IsCanceled)
            appointment.CanceledAt = now;

        dbContext.Appointments.Add(appointment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<AppointmentResponse>.Ok(AppointmentMapper.FromEntity(appointment));
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> GetListAsync(
        AppointmentListQuery query,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUser))
            return Result<IReadOnlyList<AppointmentResponse>>.Unauthorized();

        if (IsInvalidRange(query.From, query.To))
            return Result<IReadOnlyList<AppointmentResponse>>.BadRequest();

        var scopeResult = await authorizationService.ApplyListScopeAsync(
            currentUser,
            new AppointmentListScopeRequest(query.ClientId, query.TherapistUserId),
            cancellationToken);
        if (!scopeResult.IsSuccess)
            return MapFailure<IReadOnlyList<AppointmentResponse>>(scopeResult);
        return await GetListWithScopeAsync(scopeResult.Value, query, cancellationToken);
    }

    public async Task<Result<AppointmentResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUser))
            return Result<AppointmentResponse>.Unauthorized();

        var appointment = await dbContext.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (appointment == null)
            return Result<AppointmentResponse>.NotFound();

        var authResult = await authorizationService.CanAccessAppointmentAsync(
            currentUser,
            appointment,
            cancellationToken);
        if (!authResult.IsSuccess)
            return MapFailure<AppointmentResponse>(authResult);

        return Result<AppointmentResponse>.Ok(AppointmentMapper.FromEntity(appointment));
    }

    public async Task<Result<AppointmentResponse>> UpdateAsync(
        Guid id,
        AppointmentUpdateRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUser))
            return Result<AppointmentResponse>.Unauthorized();

        var appointment = await dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (appointment == null)
            return Result<AppointmentResponse>.NotFound();

        var authResult = await authorizationService.CanAccessAppointmentAsync(
            currentUser,
            appointment,
            cancellationToken);
        if (!authResult.IsSuccess)
            return MapFailure<AppointmentResponse>(authResult);

        var updated = false;

        var newStartsAt = request.StartsAt ?? appointment.StartsAt;
        var newEndsAt = request.EndsAt ?? appointment.EndsAt;
        if (newEndsAt <= newStartsAt)
            return Result<AppointmentResponse>.BadRequest();

        if (request.StartsAt.HasValue)
        {
            appointment.StartsAt = request.StartsAt.Value;
            updated = true;
        }

        if (request.EndsAt.HasValue)
        {
            appointment.EndsAt = request.EndsAt.Value;
            updated = true;
        }

        if (request.Timezone != null)
        {
            if (string.IsNullOrWhiteSpace(request.Timezone))
                return Result<AppointmentResponse>.BadRequest();

            appointment.Timezone = request.Timezone.Trim();
            updated = true;
        }

        if (request.Status != null)
        {
            if (!AppointmentStatus.TryParse(request.Status, AppointmentStatusParseMode.DefaultToScheduled, out var status))
                return Result<AppointmentResponse>.BadRequest();

            appointment.Status = status.Value;
            if (status.IsCanceled && appointment.CanceledAt == null)
                appointment.CanceledAt = DateTime.UtcNow;

            updated = true;
        }

        if (request.Mode != null)
        {
            if (!AppointmentMode.TryParse(request.Mode, out var mode))
                return Result<AppointmentResponse>.BadRequest();

            appointment.Mode = mode.Value;
            updated = true;
        }

        if (request.Location != null)
        {
            appointment.Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim();
            updated = true;
        }

        if (request.Notes != null)
        {
            appointment.Notes = request.Notes;
            updated = true;
        }

        if (request.CanceledAt.HasValue)
        {
            appointment.CanceledAt = request.CanceledAt;
            updated = true;
        }

        if (request.CancellationReason != null)
        {
            appointment.CancellationReason = string.IsNullOrWhiteSpace(request.CancellationReason)
                ? null
                : request.CancellationReason.Trim();
            updated = true;
        }

        if (!updated)
            return Result<AppointmentResponse>.BadRequest();

        appointment.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<AppointmentResponse>.Ok(AppointmentMapper.FromEntity(appointment));
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> GetMineAsync(
        AppointmentListQuery query,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUser))
            return Result<IReadOnlyList<AppointmentResponse>>.Unauthorized();

        if (string.Equals(currentUser.Role, UserRoles.Client, StringComparison.Ordinal))
        {
            var scopeResult = await authorizationService.ApplyListScopeAsync(
                currentUser,
                new AppointmentListScopeRequest(null, null),
                cancellationToken);
            if (!scopeResult.IsSuccess)
                return MapFailure<IReadOnlyList<AppointmentResponse>>(scopeResult);

            if (IsInvalidRange(query.From, query.To))
                return Result<IReadOnlyList<AppointmentResponse>>.BadRequest();
            return await GetListWithScopeAsync(scopeResult.Value, query, cancellationToken);
        }

        if (IsInvalidRange(query.From, query.To))
            return Result<IReadOnlyList<AppointmentResponse>>.BadRequest();

        var nonClientScopeResult = await authorizationService.ApplyListScopeAsync(
            currentUser,
            new AppointmentListScopeRequest(null, currentUser.UserId),
            cancellationToken);
        if (!nonClientScopeResult.IsSuccess)
            return MapFailure<IReadOnlyList<AppointmentResponse>>(nonClientScopeResult);
        return await GetListWithScopeAsync(nonClientScopeResult.Value, query, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> GetForClientAsync(
        Guid clientId,
        AppointmentListQuery query,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUser))
            return Result<IReadOnlyList<AppointmentResponse>>.Unauthorized();

        var accessResult = await authorizationService.CanAccessClientAppointmentsAsync(
            currentUser,
            clientId,
            cancellationToken);
        if (!accessResult.IsSuccess)
            return MapFailure<IReadOnlyList<AppointmentResponse>>(accessResult);

        if (IsInvalidRange(query.From, query.To))
            return Result<IReadOnlyList<AppointmentResponse>>.BadRequest();

        var scopeResult = await authorizationService.ApplyListScopeAsync(
            currentUser,
            new AppointmentListScopeRequest(clientId, null),
            cancellationToken);
        if (!scopeResult.IsSuccess)
            return MapFailure<IReadOnlyList<AppointmentResponse>>(scopeResult);
        return await GetListWithScopeAsync(scopeResult.Value, query, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> GetForTherapistAsync(
        Guid therapistUserId,
        AppointmentListQuery query,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUser))
            return Result<IReadOnlyList<AppointmentResponse>>.Unauthorized();

        var accessResult = authorizationService.CanAccessTherapistAppointments(currentUser, therapistUserId);
        if (!accessResult.IsSuccess)
            return MapFailure<IReadOnlyList<AppointmentResponse>>(accessResult);

        if (IsInvalidRange(query.From, query.To))
            return Result<IReadOnlyList<AppointmentResponse>>.BadRequest();

        var scopeResult = await authorizationService.ApplyListScopeAsync(
            currentUser,
            new AppointmentListScopeRequest(null, therapistUserId),
            cancellationToken);
        if (!scopeResult.IsSuccess)
            return MapFailure<IReadOnlyList<AppointmentResponse>>(scopeResult);
        return await GetListWithScopeAsync(scopeResult.Value, query, cancellationToken);
    }

    private async Task<Result<IReadOnlyList<AppointmentResponse>>> GetListWithScopeAsync(
        AppointmentListScope scope,
        AppointmentListQuery query,
        CancellationToken cancellationToken)
    {
        if (!TryParseStatusFilter(query.Status, out var statusFilter))
            return Result<IReadOnlyList<AppointmentResponse>>.BadRequest();

        var results = await QueryAppointmentsAsync(scope, query, statusFilter, cancellationToken);
        return Result<IReadOnlyList<AppointmentResponse>>.Ok(results);
    }

    private async Task<IReadOnlyList<AppointmentResponse>> QueryAppointmentsAsync(
        AppointmentListScope scope,
        AppointmentListQuery query,
        AppointmentStatus? statusFilter,
        CancellationToken cancellationToken)
    {
        var appointments = dbContext.Appointments.AsNoTracking();

        if (scope.ClientId.HasValue)
            appointments = appointments.Where(a => a.ClientId == scope.ClientId.Value);

        if (scope.TherapistUserId.HasValue)
            appointments = appointments.Where(a => a.TherapistUserId == scope.TherapistUserId.Value);

        if (query.From.HasValue)
            appointments = appointments.Where(a => a.StartsAt >= query.From.Value);

        if (query.To.HasValue)
            appointments = appointments.Where(a => a.StartsAt <= query.To.Value);

        if (statusFilter.HasValue)
            appointments = appointments.Where(a => a.Status == statusFilter.Value.Value);

        return await appointments
            .OrderBy(a => a.StartsAt)
            .Select(AppointmentMapper.Projection)
            .ToListAsync(cancellationToken);
    }

    private static bool TryParseStatusFilter(string? status, out AppointmentStatus? statusFilter)
    {
        statusFilter = null;
        if (string.IsNullOrWhiteSpace(status))
            return true;

        if (!AppointmentStatus.TryParse(status, AppointmentStatusParseMode.RequireValue, out var parsed))
            return false;

        statusFilter = parsed;
        return true;
    }

    private static bool IsInvalidRange(DateTime? from, DateTime? to)
        => from.HasValue && to.HasValue && from > to;

    private bool TryGetCurrentUser(out AppointmentUserContext currentUser)
    {
        currentUser = default;
        if (!currentUserAccessor.TryGetUserId(out var userId))
            return false;

        currentUserAccessor.TryGetRole(out var role);
        currentUser = new AppointmentUserContext(userId, role);
        return true;
    }

    private static Result<T> MapFailure<T>(Result result)
        => MapFailure<T>(result.Status);

    private static Result<T> MapFailure<T>(Result<AppointmentListScope> result)
        => MapFailure<T>(result.Status);

    private static Result<T> MapFailure<T>(ResultStatus status)
    {
        return status switch
        {
            ResultStatus.BadRequest => Result<T>.BadRequest(),
            ResultStatus.Unauthorized => Result<T>.Unauthorized(),
            ResultStatus.Forbidden => Result<T>.Forbidden(),
            ResultStatus.NotFound => Result<T>.NotFound(),
            ResultStatus.Conflict => Result<T>.Conflict(),
            ResultStatus.TooManyRequests => Result<T>.TooManyRequests(),
            ResultStatus.ServiceUnavailable => Result<T>.ServiceUnavailable(),
            _ => Result<T>.ServiceUnavailable()
        };
    }
}
