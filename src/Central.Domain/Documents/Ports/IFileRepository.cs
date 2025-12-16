namespace Central.Domain.Documents.Ports;

/// <summary>
/// Port for managing file storage operations.
/// </summary>
public interface IFileRepository
{
    /// <summary>
    /// Saves a file to storage.
    /// </summary>
    /// <param name="fileStream">The file content stream.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The complete file path where the file was saved.</returns>
    Task<string> SaveAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a file from storage.
    /// </summary>
    /// <param name="filePath">The complete file path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream containing the file content.</returns>
    Task<Stream> GetAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    /// <param name="filePath">The complete file path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists in storage.
    /// </summary>
    /// <param name="filePath">The complete file path.</param>
    /// <returns>True if the file exists; otherwise false.</returns>
    bool Exists(string filePath);
}
