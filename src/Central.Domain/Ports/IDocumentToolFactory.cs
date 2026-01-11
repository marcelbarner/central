namespace Central.Domain.Ports;

/// <summary>
/// Factory for resolving document tools by name.
/// </summary>
public interface IDocumentToolFactory
{
    /// <summary>
    /// Gets a document tool by its name.
    /// </summary>
    /// <param name="toolName">The name of the tool to retrieve.</param>
    /// <returns>The document tool, or null if not found.</returns>
    IDocumentTool? GetTool(string toolName);

    /// <summary>
    /// Gets all available document tools.
    /// </summary>
    /// <returns>Collection of all registered tools.</returns>
    IEnumerable<IDocumentTool> GetAllTools();
}
