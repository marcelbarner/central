using Central.Domain.DocumentTypes.Ports;

namespace Central.Domain.DocumentTypes.Services;

/// <summary>
/// Domain service implementation for document type operations.
/// </summary>
public sealed class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumentTypeRepository _documentTypeRepository;

    public DocumentTypeService(IDocumentTypeRepository documentTypeRepository)
    {
        _documentTypeRepository = documentTypeRepository;
    }

    public async Task<DocumentType> CreateAsync(string name, string? description, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim();

        // Check if document type with same name already exists
        if (await _documentTypeRepository.ExistsAsync(normalizedName, null, cancellationToken))
        {
            throw new InvalidOperationException($"A document type with the name '{normalizedName}' already exists.");
        }

        var now = DateTimeOffset.UtcNow;
        var documentType = new DocumentType
        {
            Id = 0,
            Name = normalizedName,
            Description = description?.Trim(),
            Created = now,
            Updated = now
        };

        return await _documentTypeRepository.AddAsync(documentType, cancellationToken);
    }

    public async Task<DocumentType> UpdateAsync(long id, string name, string? description, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var existingDocumentType = await _documentTypeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"Document type with ID {id} not found.");

        var normalizedName = name.Trim();

        // Check if another document type with the same name exists
        if (await _documentTypeRepository.ExistsAsync(normalizedName, id, cancellationToken))
        {
            throw new InvalidOperationException($"A document type with the name '{normalizedName}' already exists.");
        }

        var updatedDocumentType = existingDocumentType with
        {
            Name = normalizedName,
            Description = description?.Trim(),
            Updated = DateTimeOffset.UtcNow
        };

        return await _documentTypeRepository.UpdateAsync(updatedDocumentType, cancellationToken);
    }

    public async Task<DocumentType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _documentTypeRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DocumentType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _documentTypeRepository.GetAllAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await _documentTypeRepository.DeleteAsync(id, cancellationToken);
    }
}