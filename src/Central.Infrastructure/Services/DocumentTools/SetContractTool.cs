using System.Text.Json;

using Central.Domain.Ports;

using Microsoft.Extensions.Logging;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for setting the contract of a document.
/// </summary>
public sealed class SetContractTool : IDocumentTool
{
    private readonly ILogger<SetContractTool> _logger;

    public SetContractTool(ILogger<SetContractTool> logger)
    {
        _logger = logger;
    }

    public string Name => "set_contract";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var args = JsonSerializer.Deserialize<SetContractArgs>(arguments, JsonSerializerOptions.Web);
        if (args?.ContractId == null || args.ContractId <= 0)
        {
            return "Error: Valid contract ID is required";
        }

        var contract = await context.ContractRepository.GetByIdAsync(args.ContractId, cancellationToken);
        if (contract == null)
        {
            return $"Error: Contract with ID {args.ContractId} not found";
        }

        var updatedDocument = context.Document with { ContractId = args.ContractId };
        await context.DocumentRepository.UpdateAsync(updatedDocument, cancellationToken);

        _logger.LogInformation("Document {DocumentId} contract updated to: {ContractName} (ID: {ContractId})",
            context.Document.Id, contract.Name, args.ContractId);
        return $"Document contract successfully set to: {contract.Name}";
    }

    private sealed class SetContractArgs
    {
        public long ContractId { get; set; }
    }
}
