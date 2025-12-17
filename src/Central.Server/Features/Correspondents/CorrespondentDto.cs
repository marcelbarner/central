namespace Central.Server.Features.Correspondents;

public sealed record CorrespondentDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required DateTimeOffset Updated { get; init; }
}