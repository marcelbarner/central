using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.ProcessDefinitions;

public sealed class CreateProcessDefinitionEndpoint(IProcessDefinitionRepository repository)
    : Endpoint<CreateProcessDefinitionRequest, ProcessDefinitionDto>
{
    public override void Configure()
    {
        Post("/api/process-definitions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateProcessDefinitionRequest req, CancellationToken ct)
    {
        var processDefinition = new ProcessDefinition
        {
            Id = 0,
            Name = req.Name,
            Description = req.Description,
            Enabled = req.Enabled,
            TriggerState = Enum.Parse<DocumentState>(req.TriggerState),
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            Steps = req.Steps.Select((s, index) => new ProcessingStep
            {
                Id = 0,
                ProcessDefinitionId = 0,
                Name = s.Name,
                Description = s.Description,
                StepType = Enum.Parse<StepType>(s.StepType),
                Order = s.Order,
                AzureEndpoint = s.AzureEndpoint,
                AzureApiKey = s.AzureApiKey,
                AzureModelOrDeployment = s.AzureModelOrDeployment,
                Prompt = s.Prompt,
                Configuration = s.Configuration
            }).ToList()
        };

        var created = await repository.CreateAsync(processDefinition, ct);
        var dto = created.ToDto();

        await Send.CreatedAtAsync<GetProcessDefinitionByIdEndpoint>(
            new { id = created.Id },
            dto,
            cancellation: ct);
    }
}