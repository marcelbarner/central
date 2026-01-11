using Central.Domain.Documents;

using OpenAI.Chat;

namespace Central.Domain.Ports;

/// <summary>
/// Provider for building ChatTool definitions based on enabled tools.
/// </summary>
public interface IChatToolProvider
{
    /// <summary>
    /// Builds ChatTool definitions for the specified enabled tools.
    /// </summary>
    /// <param name="enabledTools">List of tool names from DocumentTool enum.</param>
    /// <returns>List of ChatTool definitions for OpenAI API.</returns>
    List<ChatTool> BuildChatTools(IEnumerable<DocumentTool> enabledTools);
}
