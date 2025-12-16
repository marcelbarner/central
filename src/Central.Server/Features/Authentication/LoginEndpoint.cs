using Central.Domain.Users;

using FastEndpoints;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

namespace Central.Server.Features.Authentication;

public sealed record LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; }

    internal sealed class Validator : Validator<LoginRequest>
    {
        public Validator(UserManager<User> userManager)
        {
            RuleFor(c => c.Username).NotEmpty();
            RuleFor(c => c.Password).NotEmpty();
        }
    }
}

public sealed record LoginResponse
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
}

public sealed class LoginEndpoint(
    SignInManager<User> signInManager,
    UserManager<User> userManager) : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(req.Username);
        if (user is null)
        {
            await Send.ErrorsAsync(statusCode: 422, cancellation: ct);
            return;
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            req.Password,
            isPersistent: req.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            await Send.ErrorsAsync(statusCode: 422, cancellation: ct);
            return;
        }

        // Update last login timestamp
        user.LastLoginAt = DateTimeOffset.UtcNow;
        await userManager.UpdateAsync(user);

        await Send.OkAsync(new LoginResponse
        {
            Username = user.UserName!,
            Email = user.Email!,
            DisplayName = user.DisplayName
        }, cancellation: ct);
    }
}