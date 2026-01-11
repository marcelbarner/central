using Central.Domain.Ports;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Factory for resolving document tools by name.
/// </summary>
public sealed class DocumentToolFactory : IDocumentToolFactory
{
    private readonly Dictionary<string, IDocumentTool> _tools;

    public DocumentToolFactory(IEnumerable<IDocumentTool> tools)
    {
        _tools = tools.ToDictionary(t => t.Name, t => t);
    }

    public IDocumentTool? GetTool(string toolName)
    {
        return _tools.GetValueOrDefault(toolName);
    }

    public IEnumerable<IDocumentTool> GetAllTools()
    {
        return _tools.Values;
    }
}
