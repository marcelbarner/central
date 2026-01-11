using System.Text.Json;

using Central.Domain.Contracts;
using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for creating a new contract.
/// </summary>
public sealed class CreateContractTool : IDocumentTool
{
    private readonly ILogger<CreateContractTool> _logger;

    public CreateContractTool(ILogger<CreateContractTool> logger)
    {
        _logger = logger;
    }

    public string Name => "create_contract";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<CreateEntityArgs>(arguments, JsonSerializerOptions.Web);
        if (args == null || string.IsNullOrWhiteSpace(args.Name))
        {
            return "Error: Name is required";
        }

        var existing = await context.ContractRepository.GetByNameAsync(args.Name, cancellationToken);
        if (existing != null)
        {
            return $"Contract '{args.Name}' already exists with ID {existing.Id}";
        }

        var now = DateTimeOffset.UtcNow;
        var contract = new Contract
        {
            Id = 0,
            Name = args.Name,
            Description = args.Description,
            State = ContractState.Active,
            Created = now,
            Updated = now
        };

        var created = await context.ContractRepository.AddAsync(contract, cancellationToken);
        _logger.LogInformation("Created new contract: {ContractName} (ID: {ContractId})", created.Name, created.Id);
        return $"Contract '{created.Name}' created successfully with ID {created.Id}";
    }

    private sealed class CreateEntityArgs
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
