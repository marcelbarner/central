using System.Text.Json;

using Central.Domain.DocumentTypes;
using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for creating a new document type.
/// </summary>
public sealed class CreateDocumentTypeTool : IDocumentTool
{
    private readonly ILogger<CreateDocumentTypeTool> _logger;

    public CreateDocumentTypeTool(ILogger<CreateDocumentTypeTool> logger)
    {
        _logger = logger;
    }

    public string Name => "create_document_type";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<CreateEntityArgs>(arguments, JsonSerializerOptions.Web);
        if (args == null || string.IsNullOrWhiteSpace(args.Name))
        {
            return "Error: Name is required";
        }

        var existing = await context.DocumentTypeRepository.GetByNameAsync(args.Name, cancellationToken);
        if (existing != null)
        {
            return $"Document type '{args.Name}' already exists with ID {existing.Id}";
        }

        var now = DateTimeOffset.UtcNow;
        var documentType = new DocumentType
        {
            Id = 0,
            Name = args.Name,
            Description = args.Description,
            Created = now,
            Updated = now
        };

        var created = await context.DocumentTypeRepository.AddAsync(documentType, cancellationToken);
        _logger.LogInformation("Created new document type: {DocumentTypeName} (ID: {DocumentTypeId})", created.Name, created.Id);
        return $"Document type '{created.Name}' created successfully with ID {created.Id}";
    }

    private sealed class CreateEntityArgs
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
