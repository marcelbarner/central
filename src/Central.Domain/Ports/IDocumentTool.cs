namespace Central.Domain.Ports;

/// <summary>
/// Interface for a document tool that can be called by AI to perform actions on documents.
/// </summary>
public interface IDocumentTool
{
    /// <summary>
    /// Gets the unique name of the tool.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Executes the tool with the provided arguments and document context.
    /// </summary>
    /// <param name="arguments">JSON string containing the tool arguments.</param>
    /// <param name="context">The document tool context containing document and repositories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result message describing the action taken.</returns>
    Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default);
}
