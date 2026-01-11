using Central.Domain.Documents;
using Central.Domain.Ports;

namespace Central.Infrastructure.Services;

/// <summary>
/// Factory for creating task executers based on task type.
/// </summary>
public sealed class TaskExecuterFactory : ITaskExecuterFactory
{
    private readonly OpenAITaskExecuter _openAIExecuter;
    private readonly DocumentIntelligenceTaskExecuter _documentIntelligenceExecuter;
    private readonly WaitTaskExecuter _waitExecuter;

    public TaskExecuterFactory(
        OpenAITaskExecuter openAIExecuter,
        DocumentIntelligenceTaskExecuter documentIntelligenceExecuter,
        WaitTaskExecuter waitExecuter)
    {
        _openAIExecuter = openAIExecuter;
        _documentIntelligenceExecuter = documentIntelligenceExecuter;
        _waitExecuter = waitExecuter;
    }

    /// <inheritdoc />
    public ITaskExecuter GetExecuter(TaskType taskType)
    {
        return taskType switch
        {
            TaskType.AzureOpenAI => _openAIExecuter,
            TaskType.AzureDocumentIntelligence => _documentIntelligenceExecuter,
            _ => throw new NotSupportedException($"Task type {taskType} is not supported.")
        };
    }

    /// <inheritdoc />
    public ITaskExecuter GetWaitExecuter()
    {
        return _waitExecuter;
    }
}
