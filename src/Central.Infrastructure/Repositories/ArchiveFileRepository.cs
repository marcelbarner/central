using Central.Domain.Documents.Ports;
using Central.Infrastructure.Configuration;

using Microsoft.Extensions.Options;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing archive file storage.
/// </summary>
public sealed class ArchiveFileRepository : FileRepositoryBase, IArchiveFileRepository
{
    public ArchiveFileRepository(IOptions<FileSystemConfiguration> configuration)
        : base(configuration, "archives")
    {
    }
}