using Central.Domain.Users;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;

namespace Central.Server.Features.Authentication;

public sealed class LogoutEndpoint(SignInManager<User> signInManager) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/api/auth/logout");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await signInManager.SignOutAsync();
        await Send.NoContentAsync(ct);
    }
}