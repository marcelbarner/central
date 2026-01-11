using System.Text.Json;

using Central.Domain.Ports;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for getting document details by ID.
/// </summary>
public sealed class GetDocumentTool : IDocumentTool
{
    public string Name => "get_document";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<GetDocumentArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.DocumentId == null || args.DocumentId <= 0)
        {
            return "Error: Valid document ID is required";
        }

        var doc = await context.DocumentRepository.GetByIdAsync(args.DocumentId, cancellationToken);
        if (doc == null)
        {
            return $"Error: Document with ID {args.DocumentId} not found";
        }

        var result = JsonSerializer.Serialize(new
        {
            doc.Id,
            doc.Title,
            doc.DocumentDate,
            doc.DocumentTypeId,
            doc.CorrespondentId,
            doc.ContractId,
            doc.TagIds,
            doc.State,
            doc.Added,
            doc.Updated
        }, new JsonSerializerOptions { WriteIndented = true });

        return $"Document details:\n{result}";
    }

    private sealed class GetDocumentArgs
    {
        public long DocumentId { get; set; }
    }
}
