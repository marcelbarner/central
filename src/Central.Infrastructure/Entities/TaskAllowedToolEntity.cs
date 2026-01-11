using Central.Domain.Documents;

namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing an allowed tool for a task.
/// Owned by TaskEntity.
/// </summary>
public sealed class TaskAllowedToolEntity
{
    /// <summary>
    /// Gets or sets the tool type.
    /// </summary>
    public DocumentTool Tool { get; set; }
}
