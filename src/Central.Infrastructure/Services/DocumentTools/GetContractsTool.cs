using System.Text.Json;

using Central.Domain.Ports;

namespace Central.Infrastructure.Services.DocumentTools;

/// <summary>
/// Tool for getting all contracts.
/// </summary>
public sealed class GetContractsTool : IDocumentTool
{
    public string Name => "get_contracts";

    public async Task<string> ExecuteAsync(string arguments, DocumentToolContext context, CancellationToken cancellationToken = default)
    {
        var contracts = await context.ContractRepository.GetAllAsync(cancellationToken);
        var contractList = contracts.Select(c => new { c.Id, c.Name }).ToList();

        var result = JsonSerializer.Serialize(contractList, new JsonSerializerOptions { WriteIndented = true });
        return $"Available contracts ({contractList.Count}):\n{result}";
    }
}
