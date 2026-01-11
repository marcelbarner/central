using System.Text.Json;

using Central.Domain.Correspondents;
using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for creating a new correspondent.
/// </summary>
public sealed class CreateCorrespondentTool : IDocumentTool
{
    private readonly ILogger<CreateCorrespondentTool> _logger;

    public CreateCorrespondentTool(ILogger<CreateCorrespondentTool> logger)
    {
        _logger = logger;
    }

    public string Name => "create_correspondent";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<CreateEntityArgs>(arguments, JsonSerializerOptions.Web);
        if (args == null || string.IsNullOrWhiteSpace(args.Name))
        {
            return "Error: Name is required";
        }

        var existing = await context.CorrespondentRepository.GetByNameAsync(args.Name, cancellationToken);
        if (existing != null)
        {
            return $"Correspondent '{args.Name}' already exists with ID {existing.Id}";
        }

        var now = DateTimeOffset.UtcNow;
        var correspondent = new Correspondent
        {
            Id = 0,
            Name = args.Name,
            Description = args.Description,
            Created = now,
            Updated = now
        };

        var created = await context.CorrespondentRepository.AddAsync(correspondent, cancellationToken);
        _logger.LogInformation("Created new correspondent: {CorrespondentName} (ID: {CorrespondentId})", created.Name, created.Id);
        return $"Correspondent '{created.Name}' created successfully with ID {created.Id}";
    }

    private sealed class CreateEntityArgs
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
