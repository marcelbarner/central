using Central.Domain.Users;
using Central.Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Central.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Infrastructure layer services including database context and Identity stores.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Register DbContext with PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register Identity Core with stores
        services.AddIdentityCore<User>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<long>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
}
