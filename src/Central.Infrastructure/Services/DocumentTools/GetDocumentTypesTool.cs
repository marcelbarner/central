using System.Text.Json;

using Central.Domain.Ports;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for getting all document types.
/// </summary>
public sealed class GetDocumentTypesTool : IDocumentTool
{
    public string Name => "get_document_types";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var documentTypes = await context.DocumentTypeRepository.GetAllAsync(cancellationToken);
        var typeList = documentTypes.Select(dt => new { dt.Id, dt.Name }).ToList();

        var result = JsonSerializer.Serialize(typeList, new JsonSerializerOptions { WriteIndented = true });
        return $"Available document types ({typeList.Count}):\n{result}";
    }
}
