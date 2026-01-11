using System.Text.Json;

using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for setting the tags of a document.
/// </summary>
public sealed class SetTagsTool : IDocumentTool
{
    private readonly ILogger<SetTagsTool> _logger;

    public SetTagsTool(ILogger<SetTagsTool> logger)
    {
        _logger = logger;
    }

    public string Name => "set_tags";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<SetTagsArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.TagIds == null)
        {
            return "Error: Tag IDs array is required";
        }

        var allTags = await context.TagRepository.GetAllAsync(cancellationToken);
        var validTagIds = allTags.Select(t => t.Id).ToHashSet();
        var invalidIds = args.TagIds.Where(id => !validTagIds.Contains(id)).ToList();

        if (invalidIds.Count > 0)
        {
            return $"Error: Invalid tag IDs: {string.Join(", ", invalidIds)}";
        }

        var updatedDocument = context.Document with { TagIds = args.TagIds.ToArray() };
        await context.DocumentRepository.UpdateAsync(updatedDocument, cancellationToken);

        var tagNames = allTags.Where(t => args.TagIds.Contains(t.Id)).Select(t => t.Name).ToList();
        _logger.LogInformation("Document {DocumentId} tags updated to: {TagNames}",
            context.Document.Id, string.Join(", ", tagNames));
        return $"Document tags successfully set to: {string.Join(", ", tagNames)}";
    }

    private sealed class SetTagsArgs
    {
        public List<long> TagIds { get; set; } = new();
    }
}
