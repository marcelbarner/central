using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Tasks;

public sealed class UpdateTaskEndpoint(ITaskRepository repository)
    : Endpoint<UpdateTaskRequest, TaskDto>
{
    public override void Configure()
    {
        Put("/api/tasks/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateTaskRequest req, CancellationToken ct)
    {
        var id = Route<long>("id");
        var existing = await repository.GetByIdAsync(id, ct);

        if (existing == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var updated = existing with
        {
            Name = req.Name,
            Description = req.Description,
            TaskType = Enum.Parse<TaskType>(req.TaskType),
            Enabled = req.Enabled,
            Configuration = new TaskConfiguration
            {
                AzureEndpoint = req.Configuration.AzureEndpoint,
                AzureApiKey = req.Configuration.AzureApiKey,
                AzureModelOrDeployment = req.Configuration.AzureModelOrDeployment,
                Prompt = req.Configuration.Prompt,
                Temperature = req.Configuration.Temperature,
                MaxTokens = req.Configuration.MaxTokens,
                Capabilities = req.Configuration.Capabilities,
                DocumentIntelligenceOptions = req.Configuration.DocumentIntelligenceOptions
            },
            Updated = DateTimeOffset.UtcNow
        };

        var result = await repository.UpdateAsync(updated, ct);
        await Send.OkAsync(result.ToDto(), ct);
    }
}
