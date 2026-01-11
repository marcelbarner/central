using System.Text.Json;

using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for setting the correspondent of a document.
/// </summary>
public sealed class SetCorrespondentTool : IDocumentTool
{
    private readonly ILogger<SetCorrespondentTool> _logger;

    public SetCorrespondentTool(ILogger<SetCorrespondentTool> logger)
    {
        _logger = logger;
    }

    public string Name => "set_correspondent";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<SetCorrespondentArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.CorrespondentId == null || args.CorrespondentId <= 0)
        {
            return "Error: Valid correspondent ID is required";
        }

        var correspondent = await context.CorrespondentRepository.GetByIdAsync(args.CorrespondentId, cancellationToken);
        if (correspondent == null)
        {
            return $"Error: Correspondent with ID {args.CorrespondentId} not found";
        }

        var updatedDocument = context.Document with { CorrespondentId = args.CorrespondentId };
        await context.DocumentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} correspondent updated to: {CorrespondentName} (ID: {CorrespondentId})",
            context.Document.Id, correspondent.Name, args.CorrespondentId);
        return $"Document correspondent successfully set to: {correspondent.Name}";
    }

    private sealed class SetCorrespondentArgs
    {
        public long CorrespondentId { get; set; }
    }
}
