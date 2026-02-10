using backend.Auth.Identity;

namespace backend.Services.Appointments.Authorization;

public static class AppointmentRolePolicy
{
    public static bool CanManageAll(string? role)
        => string.Equals(role, UserRoles.Owner, StringComparison.Ordinal) ||
           string.Equals(role, UserRoles.Admin, StringComparison.Ordinal) ||
           string.Equals(role, UserRoles.OrgManager, StringComparison.Ordinal) ||
           string.Equals(role, UserRoles.ClinicalManager, StringComparison.Ordinal);
}
