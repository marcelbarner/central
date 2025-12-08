# Testing Strategy

## Test Structure

Each production project has a corresponding test project:

* `Central.Domain.Tests` → Unit tests for domain logic
* `Central.Infrastructure.Tests` → Integration tests for data access
* `Central.Server.Tests` → Unit tests for endpoints
* `Central.ArchitectureTests` → Architecture rule validation
* `Central.AcceptanceTests` → End-to-end tests with Reqnroll

## Testing Tools

* **Test Framework**: xUnit v3
* **Assertions**: AwesomeAssertions
* **Mocking**: FakeItEasy
* **BDD**: Reqnroll with Gherkin syntax
* **Architecture**: ArchUnitNET

## Test Pattern (Arrange-Act-Assert)

```csharp
[Fact]
public async Task Should_ReturnGreeting_When_ValidRequest()
{
    // Arrange
    var request = new HelloRequest { Name = "World" };
    
    // Act
    var result = await sut.HandleAsync(request);
    
    // Assert
    result.Should().Be("Hello World");
}
```

## Coverage Requirement

Minimum 80% code coverage across all projects.
