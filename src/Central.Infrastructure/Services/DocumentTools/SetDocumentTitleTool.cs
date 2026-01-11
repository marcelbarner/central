using System.Text.Json;

using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for setting the title of a document.
/// </summary>
public sealed class SetDocumentTitleTool : IDocumentTool
{
    private readonly ILogger<SetDocumentTitleTool> _logger;

    public SetDocumentTitleTool(ILogger<SetDocumentTitleTool> logger)
    {
        _logger = logger;
    }

    public string Name => "set_document_title";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<SetDocumentTitleArgs>(arguments, JsonSerializerOptions.Web);
        if (args == null || string.IsNullOrWhiteSpace(args.Title))
        {
            return "Error: Title cannot be empty";
        }

        var updatedDocument = context.Document with { Title = args.Title };
        await context.DocumentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} title updated to: {Title}", context.Document.Id, args.Title);
        return $"Document title successfully updated to: {args.Title}";
    }

    private sealed class SetDocumentTitleArgs
    {
        public string Title { get; set; } = string.Empty;
    }
}
