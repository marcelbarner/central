using System.Text.Json;

using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for setting the content of a document.
/// </summary>
public sealed class SetContentTool : IDocumentTool
{
    private readonly ILogger<SetContentTool> _logger;

    public SetContentTool(ILogger<SetContentTool> logger)
    {
        _logger = logger;
    }

    public string Name => "set_content";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<SetContentArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.Content == null)
        {
            return "Error: Content cannot be null";
        }

        var updatedDocument = context.Document with { Content = args.Content };
        await context.DocumentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} content updated ({Length} characters)",
            context.Document.Id, args.Content.Length);
        return $"Document content successfully updated ({args.Content.Length} characters)";
    }

    private sealed class SetContentArgs
    {
        public string Content { get; set; } = string.Empty;
    }
}
