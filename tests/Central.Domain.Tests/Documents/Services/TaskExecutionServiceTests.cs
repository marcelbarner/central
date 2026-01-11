using AwesomeAssertions;

using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Domain.Documents.Services;
using Central.Domain.Ports;

using FakeItEasy;

using Microsoft.Extensions.Logging;

namespace Central.Domain.Tests.Documents.Services;

public sealed class TaskExecutionServiceTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskExecutionRepository _taskExecutionRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly ITaskExecuterFactory _taskExecuterFactory;
    private readonly ILogger<TaskExecutionService> _logger;
    private readonly TaskExecutionService _service;

    public TaskExecutionServiceTests()
    {
        _taskRepository = A.Fake<ITaskRepository>();
        _taskExecutionRepository = A.Fake<ITaskExecutionRepository>();
        _documentRepository = A.Fake<IDocumentRepository>();
        _taskExecuterFactory = A.Fake<ITaskExecuterFactory>();
        _logger = A.Fake<ILogger<TaskExecutionService>>();

        _service = new TaskExecutionService(
            _taskRepository,
            _taskExecutionRepository,
            _documentRepository,
            _taskExecuterFactory,
            _logger);
    }

    [Fact]
    public async Task ExecuteTaskAsync_WithValidTaskAndDocument_CreatesExecution()
    {
        // Arrange
        var task = CreateTestTask();
        var document = CreateTestDocument();
        var fakeExecuter = A.Fake<ITaskExecuter>();

        A.CallTo(() => _taskRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _taskExecuterFactory.GetExecuter(task.TaskType))
            .Returns(fakeExecuter);
        A.CallTo(() => fakeExecuter.ExecuteAsync(A<TaskExecutionContext>._, A<CancellationToken>._))
            .Returns("{\"status\": \"success\"}");
        A.CallTo(() => _taskExecutionRepository.CreateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te with { Id = 100 });
        A.CallTo(() => _taskExecutionRepository.UpdateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te);

        // Act
        var result = await _service.ExecuteTaskAsync(1, 1, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TaskId.Should().Be(1);
        result.DocumentId.Should().Be(1);
        result.PipelineExecutionId.Should().BeNull();
        result.Status.Should().Be(ExecutionStatus.Completed);
        result.StartedAt.Should().NotBeNull();
        result.CompletedAt.Should().NotBeNull();

        A.CallTo(() => _taskExecutionRepository.CreateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _taskExecutionRepository.UpdateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExecuteTaskAsync_WithPipelineExecutionId_LinksExecution()
    {
        // Arrange
        var task = CreateTestTask();
        var document = CreateTestDocument();

        A.CallTo(() => _taskRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _taskExecutionRepository.CreateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te with { Id = 100 });
        A.CallTo(() => _taskExecutionRepository.UpdateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te);

        // Act
        var result = await _service.ExecuteTaskAsync(1, 1, 50, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PipelineExecutionId.Should().Be(50);
        result.Status.Should().Be(ExecutionStatus.Completed);
    }

    [Fact]
    public async Task ExecuteTaskAsync_WithNonExistentTask_ThrowsInvalidOperationException()
    {
        // Arrange
        A.CallTo(() => _taskRepository.GetByIdAsync(999, A<CancellationToken>._))
            .Returns((ProcessingTask?)null);

        // Act
        var act = async () => await _service.ExecuteTaskAsync(999, 1, null, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task ExecuteTaskAsync_WithNonExistentDocument_ThrowsInvalidOperationException()
    {
        // Arrange
        var task = CreateTestTask();
        A.CallTo(() => _taskRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _documentRepository.GetByIdAsync(999, A<CancellationToken>._))
            .Returns((Document?)null);

        // Act
        var act = async () => await _service.ExecuteTaskAsync(1, 999, null, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task ExecuteTaskAsync_WhenExceptionOccurs_SetsFailedStatus()
    {
        // Arrange
        var task = CreateTestTask();
        var document = CreateTestDocument();

        A.CallTo(() => _taskRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _taskExecutionRepository.CreateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te with { Id = 100 });
        A.CallTo(() => _taskExecutionRepository.UpdateAsync(A<TaskExecution>.That.Matches(e => e.Status == ExecutionStatus.Running), A<CancellationToken>._))
            .ThrowsAsync(new Exception("Simulated error"));
        A.CallTo(() => _taskExecutionRepository.UpdateAsync(A<TaskExecution>.That.Matches(e => e.Status == ExecutionStatus.Failed), A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te);

        // Act
        var result = await _service.ExecuteTaskAsync(1, 1, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ExecutionStatus.Failed);
        result.ErrorMessage.Should().Contain("Simulated error");
        result.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteTaskAsync_WithAzureOpenAITask_ExecutesSuccessfully()
    {
        // Arrange
        var task = CreateTestTask(TaskType.AzureOpenAI);
        var document = CreateTestDocument();

        A.CallTo(() => _taskRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _taskExecutionRepository.CreateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te with { Id = 100 });
        A.CallTo(() => _taskExecutionRepository.UpdateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te);

        // Act
        var result = await _service.ExecuteTaskAsync(1, 1, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ExecutionStatus.Completed);
        result.Result.Should().Contain("Placeholder");
    }

    [Fact]
    public async Task ExecuteTaskAsync_WithAzureDocumentIntelligenceTask_ExecutesSuccessfully()
    {
        // Arrange
        var task = CreateTestTask(TaskType.AzureDocumentIntelligence);
        var document = CreateTestDocument();

        A.CallTo(() => _taskRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _taskExecutionRepository.CreateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te with { Id = 100 });
        A.CallTo(() => _taskExecutionRepository.UpdateAsync(A<TaskExecution>._, A<CancellationToken>._))
            .ReturnsLazily((TaskExecution te, CancellationToken _) => te);

        // Act
        var result = await _service.ExecuteTaskAsync(1, 1, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ExecutionStatus.Completed);
        result.Result.Should().Contain("Placeholder");
    }

    private static ProcessingTask CreateTestTask(TaskType taskType = TaskType.AzureOpenAI)
    {
        return new ProcessingTask
        {
            Id = 1,
            Name = "Test Task",
            Description = "Test Description",
            TaskType = taskType,
            Enabled = true,
            Configuration = new TaskConfiguration
            {
                AzureEndpoint = "https://test.openai.azure.com",
                AzureApiKey = "test-key",
                AzureModelOrDeployment = "gpt-4",
                Prompt = "Test prompt"
            },
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow
        };
    }

    private static Document CreateTestDocument()
    {
        return new Document
        {
            Id = 1,
            Title = "Test Document",
            State = DocumentState.Imported,
            Added = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow
        };
    }
}
