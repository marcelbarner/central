using System.Text.Json;

using Central.Domain.Ports;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for getting all tags.
/// </summary>
public sealed class GetTagsTool : IDocumentTool
{
    public string Name => "get_tags";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var tags = await context.TagRepository.GetAllAsync(cancellationToken);
        var tagList = tags.Select(t => new { t.Id, t.Name }).ToList();

        var result = JsonSerializer.Serialize(tagList, new JsonSerializerOptions { WriteIndented = true });
        return $"Available tags ({tagList.Count}):\n{result}";
    }
}
