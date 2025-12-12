using Central.Domain.Users;
using Central.Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Central.Server.Infrastructure;

/// <summary>
/// Background service that applies database migrations and seeds initial data on startup.
/// </summary>
public sealed class DatabaseInitializerService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseInitializerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting database initialization...");

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Apply migrations
            logger.LogInformation("Applying database migrations...");
            await dbContext.Database.MigrateAsync(stoppingToken);
            logger.LogInformation("Database migrations applied successfully");

            // Seed initial data
            await SeedDataAsync(userManager, stoppingToken);

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization");
            throw;
        }
    }

    private async Task SeedDataAsync(UserManager<User> userManager, CancellationToken stoppingToken)
    {
        // Check if we need to seed a default user
        if (await userManager.Users.AnyAsync(stoppingToken))
        {
            logger.LogInformation("Users already exist, skipping seed data");
            return;
        }

        logger.LogInformation("Seeding initial user data...");

        var testUser = new User
        {
            UserName = "testuser",
            Email = "testuser@example.com",
            DisplayName = "Test User",
            EmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await userManager.CreateAsync(testUser, "Test123!");
        if (result.Succeeded)
        {
            logger.LogInformation("Test user created successfully: {Username}", testUser.UserName);
        }
        else
        {
            logger.LogError("Failed to create test user: {Errors}", 
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
