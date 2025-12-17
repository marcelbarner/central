namespace Central.Server.Features.DocumentTypes;

public sealed record DocumentTypeDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required DateTimeOffset Updated { get; init; }
}