using System.Text.Json;

using Central.Domain.Documents;
using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for getting similar documents with their titles.
/// </summary>
public sealed class GetSimilarDocumentsTool : IDocumentTool
{
    private readonly ILogger<GetSimilarDocumentsTool> _logger;

    public GetSimilarDocumentsTool(ILogger<GetSimilarDocumentsTool> logger)
    {
        _logger = logger;
    }

    public string Name => "get_similar_documents";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<GetSimilarDocumentsArgs>(arguments, JsonSerializerOptions.Web);
        var limit = Math.Min(args?.Limit ?? 10, 50);

        var allDocuments = await context.DocumentRepository.GetAllAsync(cancellationToken);
        var filteredDocuments = allDocuments
            .Where(d => d.State == DocumentState.Processed ||
                       d.State == DocumentState.Review ||
                       d.State == DocumentState.Approved)
            .AsEnumerable();

        if (args?.DocumentTypeId > 0)
        {
            filteredDocuments = filteredDocuments.Where(d => d.DocumentTypeId == args.DocumentTypeId);
        }

        if (args?.CorrespondentId > 0)
        {
            filteredDocuments = filteredDocuments.Where(d => d.CorrespondentId == args.CorrespondentId);
        }

        var documents = filteredDocuments
            .OrderByDescending(d => d.Added)
            .Take(limit)
            .Select(d => new
            {
                d.Id,
                d.Title,
                d.DocumentDate,
                d.DocumentTypeId,
                d.CorrespondentId
            })
            .ToList();

        var result = JsonSerializer.Serialize(documents, new JsonSerializerOptions { WriteIndented = true });
        return $"Found {documents.Count} similar documents:\n{result}";
    }

    private sealed class GetSimilarDocumentsArgs
    {
        public long? DocumentTypeId { get; set; }
        public long? CorrespondentId { get; set; }
        public int Limit { get; set; } = 10;
    }
}
