using System.Text.Json;

using Central.Domain.Ports;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for getting all correspondents.
/// </summary>
public sealed class GetCorrespondentsTool : IDocumentTool
{
    public string Name => "get_correspondents";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var correspondents = await context.CorrespondentRepository.GetAllAsync(cancellationToken);
        var correspondentList = correspondents.Select(c => new { c.Id, c.Name }).ToList();

        var result = JsonSerializer.Serialize(correspondentList, new JsonSerializerOptions { WriteIndented = true });
        return $"Available correspondents ({correspondentList.Count}):\n{result}";
    }
}
