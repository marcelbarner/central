namespace Central.Server.Features.Documents;

public sealed record DocumentDto
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public DateTimeOffset? DocumentDate { get; init; }
    public string? Content { get; init; }
    public DocumentFileDto? OriginalFile { get; init; }
    public DocumentFileDto? ArchiveFile { get; init; }
    public DocumentFileDto? Thumbnail { get; init; }
    public required DateTimeOffset Added { get; init; }
    public required DateTimeOffset Updated { get; init; }
    public long? AddedById { get; init; }
    public long? UpdatedById { get; init; }
    public long? DocumentTypeId { get; init; }
    public long? CorrespondentId { get; init; }
    public IReadOnlyCollection<long> TagIds { get; init; } = Array.Empty<long>();
}

public sealed record DocumentFileDto
{
    public required string FileName { get; init; }
    public required string FilePath { get; init; }
}