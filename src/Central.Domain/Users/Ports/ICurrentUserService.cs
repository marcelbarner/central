namespace Central.Domain.Users.Ports;

/// <summary>
/// Service port for retrieving the current authenticated user.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the ID of the currently authenticated user.
    /// </summary>
    /// <returns>The user ID if authenticated; otherwise, null.</returns>
    Task<long?> GetCurrentUserIdAsync(CancellationToken cancellationToken = default);
}