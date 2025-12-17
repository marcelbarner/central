using Central.Domain.Tags.Ports;

namespace Central.Domain.Tags.Services;

/// <summary>
/// Domain service implementation for tag operations.
/// </summary>
public sealed class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Tag> CreateAsync(string name, string? description, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim();

        // Check if tag with same name already exists
        if (await _tagRepository.ExistsAsync(normalizedName, null, cancellationToken))
        {
            throw new InvalidOperationException($"A tag with the name '{normalizedName}' already exists.");
        }

        var now = DateTimeOffset.UtcNow;
        var tag = new Tag
        {
            Id = 0,
            Name = normalizedName,
            Description = description?.Trim(),
            Created = now,
            Updated = now
        };

        return await _tagRepository.AddAsync(tag, cancellationToken);
    }

    public async Task<Tag> UpdateAsync(long id, string name, string? description, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var existingTag = await _tagRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Tag with ID {id} not found.");

        var normalizedName = name.Trim();

        // Check if another tag with the same name exists
        if (await _tagRepository.ExistsAsync(normalizedName, id, cancellationToken))
        {
            throw new InvalidOperationException($"A tag with the name '{normalizedName}' already exists.");
        }

        var updatedTag = existingTag with
        {
            Name = normalizedName,
            Description = description?.Trim(),
            Updated = DateTimeOffset.UtcNow
        };

        return await _tagRepository.UpdateAsync(updatedTag, cancellationToken);
    }

    public async Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _tagRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _tagRepository.GetAllAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await _tagRepository.DeleteAsync(id, cancellationToken);
    }
}
