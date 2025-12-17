using Central.Domain.Documents.Ports;
using Central.Domain.Users;
using Central.Domain.Users.Ports;

namespace Central.Domain.Documents.Services;

/// <summary>
/// Domain service implementation for document operations.
/// Handles business logic including automatic archive and thumbnail generation.
/// </summary>
public sealed class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IOriginalFileRepository _originalFileRepository;
    private readonly IArchiveFileRepository _archiveFileRepository;
    private readonly IThumbnailFileRepository _thumbnailFileRepository;
    private readonly ICurrentUserService _currentUserService;

    public DocumentService(
        IDocumentRepository documentRepository,
        IOriginalFileRepository originalFileRepository,
        IArchiveFileRepository archiveFileRepository,
        IThumbnailFileRepository thumbnailFileRepository,
        ICurrentUserService currentUserService)
    {
        _documentRepository = documentRepository;
        _originalFileRepository = originalFileRepository;
        _archiveFileRepository = archiveFileRepository;
        _thumbnailFileRepository = thumbnailFileRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Document> CreateFromFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var title = Path.GetFileNameWithoutExtension(fileName);
        return await CreateAsync(title, null, null, fileStream, fileName, Array.Empty<long>(), cancellationToken);
    }

    public async Task<Document> CreateAsync(
        string title,
        DateTimeOffset? documentDate,
        string? content,
        Stream originalFileStream,
        string originalFileName,
        IReadOnlyCollection<long> tagIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(originalFileStream);
        ArgumentException.ThrowIfNullOrEmpty(originalFileName);

        var now = DateTimeOffset.UtcNow;
        var currentUserId = await _currentUserService.GetCurrentUserIdAsync(cancellationToken);

        // Save original file
        var originalPath = await _originalFileRepository.SaveAsync(originalFileStream, originalFileName, cancellationToken);
        var originalFile = new DocumentFile(originalFileName, originalPath);

        // Create archive file (copy of original)
        originalFileStream.Position = 0;
        using var archiveStream = new MemoryStream();
        await originalFileStream.CopyToAsync(archiveStream, cancellationToken);
        archiveStream.Position = 0;
        
        var archivePath = await _archiveFileRepository.SaveAsync(archiveStream, originalFileName, cancellationToken);
        var archiveFile = new DocumentFile(originalFileName, archivePath);

        // Create thumbnail (simplified - in real scenario, you'd generate an actual thumbnail)
        // For now, we'll create a placeholder thumbnail
        DocumentFile? thumbnail = null;
        if (IsPdf(originalFileName))
        {
            var thumbnailFileName = Path.ChangeExtension(originalFileName, ".jpg");
            var thumbnailContent = CreatePlaceholderThumbnail();
            using var thumbnailStream = new MemoryStream(thumbnailContent);
            
            var thumbnailPath = await _thumbnailFileRepository.SaveAsync(thumbnailStream, thumbnailFileName, cancellationToken);
            thumbnail = new DocumentFile(thumbnailFileName, thumbnailPath);
        }

        var document = new Document
        {
            Id = 0,
            Title = title,
            DocumentDate = documentDate,
            Content = content,
            OriginalFile = originalFile,
            ArchiveFile = archiveFile,
            Thumbnail = thumbnail,
            Added = now,
            Updated = now,
            AddedBy = currentUserId,
            UpdatedBy = currentUserId,
            TagIds = tagIds
        };

        return await _documentRepository.AddAsync(document, cancellationToken);
    }

    public async Task<Document> UpdateAsync(
        long id,
        string title,
        DateTimeOffset? documentDate,
        string? content,
        IReadOnlyCollection<long> tagIds,
        CancellationToken cancellationToken = default)
    {
        var existingDocument = await _documentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Document with ID {id} not found.");

        var now = DateTimeOffset.UtcNow;
        var currentUserId = await _currentUserService.GetCurrentUserIdAsync(cancellationToken);

        var updatedDocument = existingDocument with
        {
            Title = title,
            DocumentDate = documentDate,
            Content = content,
            TagIds = tagIds,
            Updated = now,
            UpdatedBy = currentUserId
        };

        return await _documentRepository.UpdateAsync(updatedDocument, cancellationToken);
    }

    public async Task<Document?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _documentRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Document>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _documentRepository.GetAllAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var document = await _documentRepository.GetByIdAsync(id, cancellationToken);
        if (document == null)
        {
            return;
        }

        // Delete all associated files
        if (document.OriginalFile != null)
        {
            await _originalFileRepository.DeleteAsync(document.OriginalFile.FilePath, cancellationToken);
        }

        if (document.ArchiveFile != null)
        {
            await _archiveFileRepository.DeleteAsync(document.ArchiveFile.FilePath, cancellationToken);
        }

        if (document.Thumbnail != null)
        {
            await _thumbnailFileRepository.DeleteAsync(document.Thumbnail.FilePath, cancellationToken);
        }

        await _documentRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<Stream> GetFileAsync(long id, string fileType, CancellationToken cancellationToken = default)
    {
        var document = await _documentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Document with ID {id} not found.");

        var (file, repository) = fileType.ToLowerInvariant() switch
        {
            "original" => (document.OriginalFile, (IFileRepository)_originalFileRepository),
            "archive" => (document.ArchiveFile, (IFileRepository)_archiveFileRepository),
            "thumbnail" => (document.Thumbnail, (IFileRepository)_thumbnailFileRepository),
            _ => throw new ArgumentException($"Invalid file type: {fileType}", nameof(fileType))
        };

        if (file == null)
        {
            throw new InvalidOperationException($"Document does not have a {fileType} file.");
        }

        return await repository.GetAsync(file.FilePath, cancellationToken);
    }

    private static bool IsPdf(string fileName)
    {
        return Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    private static byte[] CreatePlaceholderThumbnail()
    {
        // Simple 1x1 pixel transparent PNG as placeholder
        return new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
            0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
            0x89, 0x00, 0x00, 0x00, 0x0A, 0x49, 0x44, 0x41,
            0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
            0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00,
            0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
            0x42, 0x60, 0x82
        };
    }
}
