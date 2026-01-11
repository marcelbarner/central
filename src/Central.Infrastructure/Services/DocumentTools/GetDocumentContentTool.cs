using Central.Domain.Ports;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for getting the content of the current document.
/// </summary>
public sealed class GetDocumentContentTool : IDocumentTool
{
    public string Name => "get_document_content";

    public Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(context.Document.Content ?? "Document has no content.");
    }
}
