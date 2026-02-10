using backend.Models;

namespace backend.Services.Users;

public interface IUserLookupService
{
    Task<User?> GetActiveUserAsync(Guid userId, UserTrackingMode trackingMode, CancellationToken cancellationToken);
    Task<User?> GetActiveUserWithClientProfileAsync(Guid userId, UserTrackingMode trackingMode, CancellationToken cancellationToken);
}
