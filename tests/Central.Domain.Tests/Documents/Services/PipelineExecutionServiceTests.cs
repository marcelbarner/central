using AwesomeAssertions;

using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Domain.Documents.Services;

using FakeItEasy;

using Microsoft.Extensions.Logging;

namespace Central.Domain.Tests.Documents.Services;

public sealed class PipelineExecutionServiceTests
{
    private readonly IPipelineRepository _pipelineRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IPipelineExecutionRepository _pipelineExecutionRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly TaskExecutionService _taskExecutionService;
    private readonly ILogger<PipelineExecutionService> _logger;
    private readonly PipelineExecutionService _service;

    public PipelineExecutionServiceTests()
    {
        _pipelineRepository = A.Fake<IPipelineRepository>();
        _taskRepository = A.Fake<ITaskRepository>();
        _pipelineExecutionRepository = A.Fake<IPipelineExecutionRepository>();
        _documentRepository = A.Fake<IDocumentRepository>();
        _taskExecutionService = A.Fake<TaskExecutionService>();
        _logger = A.Fake<ILogger<PipelineExecutionService>>();

        _service = new PipelineExecutionService(
            _pipelineRepository,
            _pipelineExecutionRepository,
            _taskRepository,
            _documentRepository,
            _taskExecutionService,
            _logger);
    }

    [Fact]
    public async Task ExecutePipelineAsync_WithValidPipelineAndDocument_CreatesExecution()
    {
        // Arrange
        var pipeline = CreateTestPipeline();
        var document = CreateTestDocument();
        var task = CreateTestTask();

        A.CallTo(() => _pipelineRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(pipeline);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _taskRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _pipelineExecutionRepository.CreateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe with { Id = 100 });
        A.CallTo(() => _pipelineExecutionRepository.UpdateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe);
        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(1, 1, 100, A<CancellationToken>._))
            .Returns(CreateTestTaskExecution());

        // Act
        var result = await _service.ExecutePipelineAsync(1, 1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PipelineId.Should().Be(1);
        result.DocumentId.Should().Be(1);
        result.Status.Should().Be(ExecutionStatus.Completed);
        result.StartedAt.Should().NotBeNull();
        result.CompletedAt.Should().NotBeNull();

        A.CallTo(() => _pipelineExecutionRepository.CreateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(1, 1, 100, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExecutePipelineAsync_WithMultipleTaskSteps_ExecutesInOrder()
    {
        // Arrange
        var pipeline = CreateTestPipelineWithMultipleSteps();
        var document = CreateTestDocument();
        var task = CreateTestTask();

        A.CallTo(() => _pipelineRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(pipeline);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _taskRepository.GetByIdAsync(A<long>._, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _pipelineExecutionRepository.CreateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe with { Id = 100 });
        A.CallTo(() => _pipelineExecutionRepository.UpdateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe);
        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(A<long>._, 1, 100, A<CancellationToken>._))
            .Returns(CreateTestTaskExecution());

        // Act
        var result = await _service.ExecutePipelineAsync(1, 1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ExecutionStatus.Completed);

        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(1, 1, 100, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(2, 1, 100, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExecutePipelineAsync_WithWaitStep_WaitsCorrectDuration()
    {
        // Arrange
        var pipeline = CreateTestPipelineWithWaitStep();
        var document = CreateTestDocument();

        A.CallTo(() => _pipelineRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(pipeline);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _pipelineExecutionRepository.CreateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe with { Id = 100 });
        A.CallTo(() => _pipelineExecutionRepository.UpdateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe);

        // Act
        var startTime = DateTimeOffset.UtcNow;
        var result = await _service.ExecutePipelineAsync(1, 1, CancellationToken.None);
        var endTime = DateTimeOffset.UtcNow;

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ExecutionStatus.Completed);
        var duration = endTime - startTime;
        duration.TotalSeconds.Should().BeGreaterThanOrEqualTo(1); // Wait step is 1 second
    }

    [Fact]
    public async Task ExecutePipelineAsync_WithNonExistentPipeline_ThrowsInvalidOperationException()
    {
        // Arrange
        A.CallTo(() => _pipelineRepository.GetByIdAsync(999, A<CancellationToken>._))
            .Returns((Pipeline?)null);

        // Act
        var act = async () => await _service.ExecutePipelineAsync(999, 1, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task ExecutePipelineAsync_WithNonExistentDocument_ThrowsInvalidOperationException()
    {
        // Arrange
        var pipeline = CreateTestPipeline();
        A.CallTo(() => _pipelineRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(pipeline);
        A.CallTo(() => _documentRepository.GetByIdAsync(999, A<CancellationToken>._))
            .Returns((Document?)null);

        // Act
        var act = async () => await _service.ExecutePipelineAsync(1, 999, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task ExecutePipelineAsync_WhenTaskFails_StopsExecution()
    {
        // Arrange
        var pipeline = CreateTestPipelineWithMultipleSteps();
        var document = CreateTestDocument();
        var task = CreateTestTask();
        var failedExecution = CreateTestTaskExecution() with { Status = ExecutionStatus.Failed };

        A.CallTo(() => _pipelineRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(pipeline);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _taskRepository.GetByIdAsync(A<long>._, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _pipelineExecutionRepository.CreateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe with { Id = 100 });
        A.CallTo(() => _pipelineExecutionRepository.UpdateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe);
        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(1, 1, 100, A<CancellationToken>._))
            .Returns(failedExecution);

        // Act
        var result = await _service.ExecutePipelineAsync(1, 1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ExecutionStatus.Failed);
        result.ErrorMessage.Should().Contain("failed");

        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(1, 1, 100, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(2, 1, 100, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task ProcessPendingDocumentsAsync_WithEnabledPipelines_ExecutesForMatchingDocuments()
    {
        // Arrange
        var pipeline = CreateTestPipeline();
        var documents = new List<Document> { CreateTestDocument() };
        var task = CreateTestTask();

        A.CallTo(() => _pipelineRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(new[] { pipeline });
        A.CallTo(() => _documentRepository.GetByStateAsync(DocumentState.Imported, A<CancellationToken>._))
            .Returns(documents);
        A.CallTo(() => _taskRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _pipelineExecutionRepository.CreateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe with { Id = 100 });
        A.CallTo(() => _pipelineExecutionRepository.UpdateAsync(A<PipelineExecution>._, A<CancellationToken>._))
            .ReturnsLazily((PipelineExecution pe, CancellationToken _) => pe);
        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(1, 1, 100, A<CancellationToken>._))
            .Returns(CreateTestTaskExecution());

        // Act
        var result = await _service.ProcessPendingDocumentsAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
        A.CallTo(() => _taskExecutionService.ExecuteTaskAsync(1, 1, 100, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessPendingDocumentsAsync_WithNoEnabledPipelines_ReturnsZero()
    {
        // Arrange
        A.CallTo(() => _pipelineRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(Array.Empty<Pipeline>());

        // Act
        var result = await _service.ProcessPendingDocumentsAsync(CancellationToken.None);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task ProcessPendingDocumentsAsync_WithPipelinesWithoutTriggerState_SkipsThem()
    {
        // Arrange
        var pipelineWithoutTrigger = CreateTestPipeline() with { TriggerState = null };

        A.CallTo(() => _pipelineRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(new[] { pipelineWithoutTrigger });

        // Act
        var result = await _service.ProcessPendingDocumentsAsync(CancellationToken.None);

        // Assert
        result.Should().Be(0);
        A.CallTo(() => _documentRepository.GetByStateAsync(A<DocumentState>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    private static Pipeline CreateTestPipeline()
    {
        return new Pipeline
        {
            Id = 1,
            Name = "Test Pipeline",
            Description = "Test Description",
            Enabled = true,
            TriggerState = DocumentState.Imported,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            Steps = new List<PipelineStep>
            {
                new()
                {
                    Id = 1,
                    PipelineId = 1,
                    Name = "Task Step 1",
                    StepType = PipelineStepType.TaskStep,
                    Order = 1,
                    TaskId = 1
                }
            }
        };
    }

    private static Pipeline CreateTestPipelineWithMultipleSteps()
    {
        return new Pipeline
        {
            Id = 1,
            Name = "Test Pipeline",
            Description = "Test Description",
            Enabled = true,
            TriggerState = DocumentState.Imported,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            Steps = new List<PipelineStep>
            {
                new()
                {
                    Id = 1,
                    PipelineId = 1,
                    Name = "Task Step 1",
                    StepType = PipelineStepType.TaskStep,
                    Order = 1,
                    TaskId = 1
                },
                new()
                {
                    Id = 2,
                    PipelineId = 1,
                    Name = "Task Step 2",
                    StepType = PipelineStepType.TaskStep,
                    Order = 2,
                    TaskId = 2
                }
            }
        };
    }

    private static Pipeline CreateTestPipelineWithWaitStep()
    {
        return new Pipeline
        {
            Id = 1,
            Name = "Test Pipeline",
            Description = "Test Description",
            Enabled = true,
            TriggerState = DocumentState.Imported,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            Steps = new List<PipelineStep>
            {
                new()
                {
                    Id = 1,
                    PipelineId = 1,
                    Name = "Wait Step",
                    StepType = PipelineStepType.WaitStep,
                    Order = 1,
                    WaitDurationSeconds = 1
                }
            }
        };
    }

    private static ProcessingTask CreateTestTask()
    {
        return new ProcessingTask
        {
            Id = 1,
            Name = "Test Task",
            Description = "Test Description",
            TaskType = TaskType.AzureOpenAI,
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

    private static TaskExecution CreateTestTaskExecution()
    {
        return new TaskExecution
        {
            Id = 50,
            TaskId = 1,
            DocumentId = 1,
            PipelineExecutionId = 100,
            Status = ExecutionStatus.Completed,
            StartedAt = DateTimeOffset.UtcNow,
            CompletedAt = DateTimeOffset.UtcNow,
            Result = "Test result"
        };
    }
}
