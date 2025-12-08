# Testing Strategy

## Test Structure

Each production project has a corresponding test project:

* `Central.Domain.Tests` → Unit tests for domain logic
* `Central.Infrastructure.Tests` → Integration tests for data access
* `Central.Server.Tests` → Unit tests for endpoints
* `Central.ArchitectureTests` → Architecture rule validation
* `Central.AcceptanceTests` → End-to-end tests with Reqnroll + Playwright

## Testing Tools

* **Test Framework**: xUnit v3
* **Assertions**: AwesomeAssertions
* **Mocking**: FakeItEasy
* **BDD**: Reqnroll with Gherkin syntax
* **Browser Automation**: Microsoft.Playwright
* **Architecture**: ArchUnitNET
* **Orchestration**: Aspire.Hosting.Testing

## Acceptance Testing

Acceptance tests validate complete user workflows:

### Backend API Tests
- Use `Aspire.Hosting.Testing` to orchestrate services
- Create HTTP clients via `fixture.App.CreateHttpClient("server")`
- Test FastEndpoints endpoints with real HTTP calls
- Verify request/response payloads and status codes

### Frontend Browser Tests
- Use `Microsoft.Playwright` for browser automation
- Launch headless Chromium for UI interaction
- Test Angular components in real browser environment
- Verify navigation, form submissions, and user interactions
- Support for screenshots and debugging with headed browsers

### Example Structure

```gherkin
Feature: User Login
    Scenario: Successfully login with valid credentials
        Given the application is running
        And I navigate to the login page
        When I enter username "ng-matero" and password "ng-matero"
        And I click the login button
        Then I should be redirected to the home page
```

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
