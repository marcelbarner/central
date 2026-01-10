using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Server.Mappers;

using FastEndpoints;

namespace Central.Server.Features.Tasks;

public sealed class CreateTaskEndpoint(ITaskRepository repository)
    : Endpoint<CreateTaskRequest, TaskDto>
{
    public override void Configure()
    {
        Post("/api/tasks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateTaskRequest req, CancellationToken ct)
    {
        var task = new ProcessingTask
        {
            Id = 0,
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
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow
        };

        var created = await repository.CreateAsync(task, ct);
        var dto = created.ToDto();

        await Send.CreatedAtAsync<GetTaskByIdEndpoint>(
            new { id = created.Id },
            dto,
            cancellation: ct);
    }
}
