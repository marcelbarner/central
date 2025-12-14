using Central.Domain.Users;
using Central.Server.Mappers;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;

namespace Central.Server.Features.Authentication;

public sealed record CurrentUserResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required List<string> Roles { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
}

public sealed class GetCurrentUserEndpoint(UserManager<User> userManager) : EndpointWithoutRequest<CurrentUserResponse>
{
    public override void Configure()
    {
        Get("/api/auth/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var roles = await userManager.GetRolesAsync(user);
        var response = user.ToCurrentUserResponse([.. roles]);

        await Send.OkAsync(response, cancellation: ct);
    }
}