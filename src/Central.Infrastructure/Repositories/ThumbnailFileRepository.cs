using Central.Domain.Documents.Ports;
using Central.Infrastructure.Configuration;

using Microsoft.Extensions.Options;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing thumbnail file storage.
/// </summary>
public sealed class ThumbnailFileRepository : FileRepositoryBase, IThumbnailFileRepository
{
    public ThumbnailFileRepository(IOptions<FileSystemConfiguration> configuration)
        : base(configuration, "thumbnails")
    {
    }
}