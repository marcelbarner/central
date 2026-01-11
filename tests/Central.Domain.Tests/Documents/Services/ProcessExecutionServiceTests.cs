using AwesomeAssertions;

using Central.Domain.Contracts.Ports;
using Central.Domain.Correspondents.Ports;
using Central.Domain.Documents;
using Central.Domain.Documents.Ports;
using Central.Domain.Documents.Services;
using Central.Domain.DocumentTypes.Ports;
using Central.Domain.Tags.Ports;

using FakeItEasy;

using Microsoft.Extensions.Logging;

namespace Central.Domain.Tests.Documents.Services;

public sealed class ProcessExecutionServiceTests
{
    private readonly IProcessDefinitionRepository _processDefinitionRepository;
    private readonly IProcessExecutionRepository _processExecutionRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly ICorrespondentRepository _correspondentRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<ProcessExecutionService> _logger;
    private readonly ProcessExecutionService _service;

    public ProcessExecutionServiceTests()
    {
        _processDefinitionRepository = A.Fake<IProcessDefinitionRepository>();
        _processExecutionRepository = A.Fake<IProcessExecutionRepository>();
        _documentRepository = A.Fake<IDocumentRepository>();
        _contractRepository = A.Fake<IContractRepository>();
        _documentTypeRepository = A.Fake<IDocumentTypeRepository>();
        _correspondentRepository = A.Fake<ICorrespondentRepository>();
        _tagRepository = A.Fake<ITagRepository>();
        _logger = A.Fake<ILogger<ProcessExecutionService>>();

        _service = new ProcessExecutionService(
            _processDefinitionRepository,
            _processExecutionRepository,
            _documentRepository,
            _contractRepository,
            _documentTypeRepository,
            _correspondentRepository,
            _tagRepository,
            _logger);
    }

    [Fact]
    public async Task ExecuteProcessAsync_WithValidInputs_CreatesExecution()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();
        var document = CreateTestDocument();

        A.CallTo(() => _processDefinitionRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(processDefinition);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _processExecutionRepository.CreateAsync(A<ProcessExecution>._, A<CancellationToken>._))
            .ReturnsLazily((ProcessExecution pe, CancellationToken _) => pe with { Id = 100 });
        A.CallTo(() => _processExecutionRepository.UpdateAsync(A<ProcessExecution>._, A<CancellationToken>._))
            .ReturnsLazily((ProcessExecution pe, CancellationToken _) => pe);
        A.CallTo(() => _documentRepository.UpdateAsync(A<Document>._, A<CancellationToken>._))
            .ReturnsLazily((Document d, CancellationToken _) => d);

        // Act
        var result = await _service.ExecuteProcessAsync(1, 1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ExecutionStatus.Completed);
        result.DocumentId.Should().Be(1);
        result.ProcessDefinitionId.Should().Be(1);

        A.CallTo(() => _processExecutionRepository.CreateAsync(
            A<ProcessExecution>._,
            A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ExecuteProcessAsync_WithNonExistentProcess_ThrowsException()
    {
        // Arrange
        A.CallTo(() => _processDefinitionRepository.GetByIdAsync(999, A<CancellationToken>._))
            .Returns((ProcessDefinition?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExecuteProcessAsync(999, 1, CancellationToken.None));

        exception.Message.Should().Contain("Process definition 999 not found");
    }

    [Fact]
    public async Task ExecuteProcessAsync_WithNonExistentDocument_ThrowsException()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();
        A.CallTo(() => _processDefinitionRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(processDefinition);
        A.CallTo(() => _documentRepository.GetByIdAsync(999, A<CancellationToken>._))
            .Returns((Document?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExecuteProcessAsync(1, 999, CancellationToken.None));

        exception.Message.Should().Contain("Document 999 not found");
    }

    [Fact]
    public async Task ExecuteProcessAsync_UpdatesDocumentStateToProcessing()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();
        var document = CreateTestDocument();

        A.CallTo(() => _processDefinitionRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(processDefinition);
        A.CallTo(() => _documentRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(document);
        A.CallTo(() => _processExecutionRepository.CreateAsync(A<ProcessExecution>._, A<CancellationToken>._))
            .ReturnsLazily((ProcessExecution pe, CancellationToken _) => pe with { Id = 100 });
        A.CallTo(() => _processExecutionRepository.UpdateAsync(A<ProcessExecution>._, A<CancellationToken>._))
            .ReturnsLazily((ProcessExecution pe, CancellationToken _) => pe);
        A.CallTo(() => _documentRepository.UpdateAsync(A<Document>._, A<CancellationToken>._))
            .ReturnsLazily((Document d, CancellationToken _) => d);

        // Act
        await _service.ExecuteProcessAsync(1, 1, CancellationToken.None);

        // Assert
        A.CallTo(() => _documentRepository.UpdateAsync(
            A<Document>.That.Matches(d => d.State == DocumentState.Processing),
            A<CancellationToken>._)).MustHaveHappened();

        A.CallTo(() => _documentRepository.UpdateAsync(
            A<Document>.That.Matches(d => d.State == DocumentState.Processed),
            A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task ProcessPendingDocumentsAsync_WithNoEnabledProcesses_ReturnsZero()
    {
        // Arrange
        A.CallTo(() => _processDefinitionRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(Array.Empty<ProcessDefinition>());

        // Act
        var result = await _service.ProcessPendingDocumentsAsync(CancellationToken.None);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task ProcessPendingDocumentsAsync_WithMatchingDocuments_CreatesExecutions()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();
        var documents = new[] { CreateTestDocument(), CreateTestDocument() with { Id = 2 } };

        A.CallTo(() => _processDefinitionRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(new[] { processDefinition });
        A.CallTo(() => _documentRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(documents);
        A.CallTo(() => _processExecutionRepository.GetByDocumentIdAsync(A<long>._, A<CancellationToken>._))
            .Returns(Array.Empty<ProcessExecution>());
        A.CallTo(() => _processDefinitionRepository.GetByIdAsync(1, A<CancellationToken>._))
            .Returns(processDefinition);
        A.CallTo(() => _documentRepository.GetByIdAsync(A<long>._, A<CancellationToken>._))
            .ReturnsLazily((long id, CancellationToken _) => documents.FirstOrDefault(d => d.Id == id));
        A.CallTo(() => _processExecutionRepository.CreateAsync(A<ProcessExecution>._, A<CancellationToken>._))
            .ReturnsLazily((ProcessExecution pe, CancellationToken _) => pe with { Id = 100 });
        A.CallTo(() => _processExecutionRepository.UpdateAsync(A<ProcessExecution>._, A<CancellationToken>._))
            .ReturnsLazily((ProcessExecution pe, CancellationToken _) => pe);
        A.CallTo(() => _documentRepository.UpdateAsync(A<Document>._, A<CancellationToken>._))
            .ReturnsLazily((Document d, CancellationToken _) => d);

        // Act
        var result = await _service.ProcessPendingDocumentsAsync(CancellationToken.None);

        // Assert
        result.Should().Be(2);
    }

    private static ProcessDefinition CreateTestProcessDefinition()
    {
        return new ProcessDefinition
        {
            Id = 1,
            Name = "Test Process",
            Description = "Test process description",
            Enabled = true,
            TriggerState = DocumentState.Imported,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
            Steps = new[]
            {
                new ProcessingStep
                {
                    Id = 1,
                    ProcessDefinitionId = 1,
                    Name = "Step 1",
                    StepType = StepType.AzureDocumentIntelligence,
                    Order = 0
                }
            }
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