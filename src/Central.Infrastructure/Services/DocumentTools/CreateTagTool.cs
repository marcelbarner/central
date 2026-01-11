using System.Text.Json;

using Central.Domain.Ports;
using Central.Domain.Tags;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for creating a new tag.
/// </summary>
public sealed class CreateTagTool : IDocumentTool
{
    private readonly ILogger<CreateTagTool> _logger;

    public CreateTagTool(ILogger<CreateTagTool> logger)
    {
        _logger = logger;
    }

    public string Name => "create_tag";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<CreateEntityArgs>(arguments, JsonSerializerOptions.Web);
        if (args == null || string.IsNullOrWhiteSpace(args.Name))
        {
            return "Error: Name is required";
        }

        var existing = await context.TagRepository.GetByNameAsync(args.Name, cancellationToken);
        if (existing != null)
        {
            return $"Tag '{args.Name}' already exists with ID {existing.Id}";
        }

        var now = DateTimeOffset.UtcNow;
        var tag = new Tag
        {
            Id = 0,
            Name = args.Name,
            Description = args.Description,
            Created = now,
            Updated = now
        };

        var created = await context.TagRepository.AddAsync(tag, cancellationToken);
        _logger.LogInformation("Created new tag: {TagName} (ID: {TagId})", created.Name, created.Id);
        return $"Tag '{created.Name}' created successfully with ID {created.Id}";
    }

    private sealed class CreateEntityArgs
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
