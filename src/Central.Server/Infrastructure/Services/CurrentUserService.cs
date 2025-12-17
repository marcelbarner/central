using System.Security.Claims;

using Central.Domain.Users.Ports;

namespace Central.Server.Infrastructure.Services;

/// <summary>
/// Service for retrieving the current authenticated user from HTTP context.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<long?> GetCurrentUserIdAsync(CancellationToken cancellationToken = default)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Task.FromResult<long?>(null);
        }

        if (long.TryParse(userIdClaim, out var userId))
        {
            return Task.FromResult<long?>(userId);
        }

        return Task.FromResult<long?>(null);
    }
}