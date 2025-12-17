namespace Central.Infrastructure.Entities;

/// <summary>
/// Entity representing a correspondent in the database.
/// </summary>
public class CorrespondentEntity
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the correspondent name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correspondent description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the documents associated with this correspondent.
    /// </summary>
    public ICollection<DocumentEntity> Documents { get; set; } = [];
}