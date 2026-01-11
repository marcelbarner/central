using Central.Domain.Contracts.Ports;
using Central.Domain.Correspondents.Ports;
using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Domain.DocumentTypes.Ports;
using Central.Domain.Tags.Ports;

namespace Central.Domain.Ports;

/// <summary>
/// Context containing all information needed for document tool execution.
/// </summary>
public sealed record DocumentToolContext
{
    /// <summary>
    /// The document being processed.
    /// </summary>
    public required Document Document { get; init; }

    /// <summary>
    /// Repository for document operations.
    /// </summary>
    public required IDocumentRepository DocumentRepository { get; init; }

    /// <summary>
    /// Repository for contract operations.
    /// </summary>
    public required IContractRepository ContractRepository { get; init; }

    /// <summary>
    /// Repository for document type operations.
    /// </summary>
    public required IDocumentTypeRepository DocumentTypeRepository { get; init; }

    /// <summary>
    /// Repository for correspondent operations.
    /// </summary>
    public required ICorrespondentRepository CorrespondentRepository { get; init; }

    /// <summary>
    /// Repository for tag operations.
    /// </summary>
    public required ITagRepository TagRepository { get; init; }
}
