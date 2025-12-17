using Central.Domain.Correspondents.Ports;
using Central.Domain.Correspondents.Services;
using Central.Domain.Documents.Ports;
using Central.Domain.Documents.Services;
using Central.Domain.DocumentTypes.Ports;
using Central.Domain.DocumentTypes.Services;
using Central.Domain.Tags.Ports;
using Central.Domain.Tags.Services;
using Central.Domain.Users;
using Central.Domain.Webhooks.Ports;
using Central.Domain.Webhooks.Services;
using Central.Infrastructure.Configuration;
using Central.Infrastructure.Persistence;
using Central.Infrastructure.Repositories;
using Central.Infrastructure.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Central.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Infrastructure layer services including database context and Identity stores.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
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

        // Configure FileSystemConfiguration
        var fileSystemSection = configuration.GetSection("FileSystem");
        var fileSystemConfig = new FileSystemConfiguration
        {
            Media = fileSystemSection["Media"] ?? "./Media"
        };
        services.AddSingleton(Options.Create(fileSystemConfig));

        // Register repositories
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IOriginalFileRepository, OriginalFileRepository>();
        services.AddScoped<IArchiveFileRepository, ArchiveFileRepository>();
        services.AddScoped<IThumbnailFileRepository, ThumbnailFileRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        services.AddScoped<ICorrespondentRepository, CorrespondentRepository>();
        services.AddScoped<IWebhookRepository, WebhookRepository>();

        // Register domain services
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        services.AddScoped<ICorrespondentService, CorrespondentService>();
        services.AddScoped<IWebhookService, WebhookService>();
        services.AddScoped<IWebhookTrigger, WebhookTrigger>();

        // Register HttpClient for webhooks
        services.AddHttpClient("WebhookClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}