using Central.Domain.Documents.Ports;
using Central.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Base repository for file storage operations.
/// </summary>
public abstract class FileRepositoryBase : IFileRepository
{
    private readonly string _storagePath;

    protected FileRepositoryBase(IOptions<FileSystemConfiguration> configuration, string subFolder)
    {
        var baseMediaPath = configuration.Value.Media;
        _storagePath = Path.Combine(baseMediaPath, subFolder);

        // Ensure directory exists
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var sanitizedFileName = SanitizeFileName(fileName);
        var uniqueFileName = GenerateUniqueFileName(sanitizedFileName);
        var filePath = Path.Combine(_storagePath, uniqueFileName);

        await using var fileStreamOut = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(fileStreamOut, cancellationToken);

        return filePath;
    }

    public Task<Stream> GetAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    public bool Exists(string filePath)
    {
        return File.Exists(filePath);
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        return sanitized;
    }

    private string GenerateUniqueFileName(string fileName)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        return $"{fileNameWithoutExtension}_{uniqueId}{extension}";
    }
}
