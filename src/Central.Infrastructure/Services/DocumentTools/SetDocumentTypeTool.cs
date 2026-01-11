using System.Text.Json;

using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for setting the document type of a document.
/// </summary>
public sealed class SetDocumentTypeTool : IDocumentTool
{
    private readonly ILogger<SetDocumentTypeTool> _logger;

    public SetDocumentTypeTool(ILogger<SetDocumentTypeTool> logger)
    {
        _logger = logger;
    }

    public string Name => "set_document_type";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<SetDocumentTypeArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.DocumentTypeId == null || args.DocumentTypeId <= 0)
        {
            return "Error: Valid document type ID is required";
        }

        var documentType = await context.DocumentTypeRepository.GetByIdAsync(args.DocumentTypeId, cancellationToken);
        if (documentType == null)
        {
            return $"Error: Document type with ID {args.DocumentTypeId} not found";
        }

        var updatedDocument = context.Document with { DocumentTypeId = args.DocumentTypeId };
        await context.DocumentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} type updated to: {DocumentTypeName} (ID: {DocumentTypeId})",
            context.Document.Id, documentType.Name, args.DocumentTypeId);
        return $"Document type successfully set to: {documentType.Name}";
    }

    private sealed class SetDocumentTypeArgs
    {
        public long DocumentTypeId { get; set; }
    }
}
