using Central.Domain.Documents.Ports;
using Central.Infrastructure.Configuration;

using Microsoft.Extensions.Options;

namespace Central.Infrastructure.Repositories;

/// <summary>
/// Repository for managing original file storage.
/// </summary>
public sealed class OriginalFileRepository : FileRepositoryBase, IOriginalFileRepository
{
    public OriginalFileRepository(IOptions<FileSystemConfiguration> configuration)
        : base(configuration, "originals")
    {
    }
}