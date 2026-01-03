using AwesomeAssertions;

using Central.Domain.Documents;
using Central.Infrastructure.Persistence;
using Central.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Central.Infrastructure.Tests.Repositories;

public sealed class ProcessDefinitionRepositoryTests : IAsyncLifetime
{
    private ApplicationDbContext _context = null!;
    private ProcessDefinitionRepository _repository = null!;

    public async ValueTask InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        await _context.Database.EnsureCreatedAsync();
        _repository = new ProcessDefinitionRepository(_context);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task CreateAsync_WithValidProcessDefinition_ReturnsCreatedEntity()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();

        // Act
        var result = await _repository.CreateAsync(processDefinition, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Process");
        result.Steps.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsProcessDefinition()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();
        var created = await _repository.CreateAsync(processDefinition, CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(created.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Name.Should().Be("Test Process");
        result.Steps.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleProcesses_ReturnsAllProcesses()
    {
        // Arrange
        await _repository.CreateAsync(CreateTestProcessDefinition(), CancellationToken.None);
        await _repository.CreateAsync(CreateTestProcessDefinition() with { Name = "Process 2" }, CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_WithModifiedProcess_UpdatesEntity()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();
        var created = await _repository.CreateAsync(processDefinition, CancellationToken.None);
        var updated = created with
        {
            Name = "Updated Process",
            Description = "Updated description"
        };

        // Act
        var result = await _repository.UpdateAsync(updated, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Process");
        result.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_RemovesProcess()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();
        var created = await _repository.CreateAsync(processDefinition, CancellationToken.None);

        // Act
        await _repository.DeleteAsync(created.Id, CancellationToken.None);

        // Assert
        var result = await _repository.GetByIdAsync(created.Id, CancellationToken.None);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEnabledByTriggerStateAsync_WithMatchingState_ReturnsProcesses()
    {
        // Arrange
        var enabledProcess = CreateTestProcessDefinition() with { Enabled = true, TriggerState = DocumentState.Imported };
        var disabledProcess = CreateTestProcessDefinition() with { Name = "Disabled", Enabled = false, TriggerState = DocumentState.Imported };
        var differentStateProcess = CreateTestProcessDefinition() with { Name = "Different State", Enabled = true, TriggerState = DocumentState.Processed };

        await _repository.CreateAsync(enabledProcess, CancellationToken.None);
        await _repository.CreateAsync(disabledProcess, CancellationToken.None);
        await _repository.CreateAsync(differentStateProcess, CancellationToken.None);

        // Act
        var result = await _repository.GetEnabledByTriggerStateAsync(DocumentState.Imported, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Test Process");
    }

    [Fact]
    public async Task UpdateAsync_WithAddedStep_PersistsNewStep()
    {
        // Arrange
        var processDefinition = CreateTestProcessDefinition();
        var created = await _repository.CreateAsync(processDefinition, CancellationToken.None);

        var newStep = new ProcessingStep
        {
            Id = 0,
            ProcessDefinitionId = created.Id,
            Name = "New Step",
            StepType = StepType.AzureOpenAI,
            Order = 1,
            AzureEndpoint = "https://test.openai.azure.com",
            AzureApiKey = "test-key",
            AzureModelOrDeployment = "gpt-4",
            Prompt = "Analyze this"
        };

        var updated = created with
        {
            Steps = created.Steps.Append(newStep).ToArray()
        };

        // Act
        var result = await _repository.UpdateAsync(updated, CancellationToken.None);

        // Assert
        result.Steps.Should().HaveCount(2);
        result.Steps.Last().Name.Should().Be("New Step");
    }

    private static ProcessDefinition CreateTestProcessDefinition()
    {
        return new ProcessDefinition
        {
            Id = 0,
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
                    Id = 0,
                    ProcessDefinitionId = 0,
                    Name = "Step 1",
                    StepType = StepType.AzureDocumentIntelligence,
                    Order = 0,
                    AzureEndpoint = "https://test.cognitiveservices.azure.com",
                    AzureApiKey = "test-key"
                }
            }
        };
    }
}