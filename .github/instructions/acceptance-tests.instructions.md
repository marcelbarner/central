---
applyTo: "tests/Central.AcceptanceTests/**/*"
---

# Acceptance Tests Instructions

## Overview

Acceptance tests validate end-to-end functionality using Reqnroll (Cucumber/Gherkin) with:
- **Backend**: Aspire.Hosting.Testing for orchestrated services
- **Frontend**: Microsoft.Playwright for browser automation
- **Assertions**: AwesomeAssertions for fluent syntax

## Project Structure

```
/tests/Central.AcceptanceTests
├── Features/                    → Gherkin feature files
│   ├── *.feature               → Scenarios in Gherkin syntax
│   └── *.feature.cs            → Auto-generated code-behind
├── StepDefinitions/            → Step implementation
│   └── *Steps.cs               → C# step definitions
└── Fixture/                    → Test infrastructure
    └── EnvironmentFixture.cs   → Aspire app orchestration
```

## Writing Acceptance Tests

### 1. Create Feature File

Place in `Features/` using Gherkin syntax:

```gherkin
Feature: Feature Name
    As a [role]
    I want to [action]
    So that [benefit]

Scenario: Scenario description
    Given [precondition]
    When [action]
    Then [expected result]
```

### 2. Implement Step Definitions

Create in `StepDefinitions/` with `[Binding]` attribute:

```csharp
using Aspire.Hosting.Testing;
using AwesomeAssertions;
using Central.AcceptanceTests.Fixture;
using Reqnroll;

namespace Central.AcceptanceTests.StepDefinitions;

[Binding]
public class FeatureSteps(EnvironmentFixture fixture)
{
    [Given(@"some precondition")]
    public void GivenSomePrecondition()
    {
        // Setup
    }

    [When(@"some action")]
    public async Task WhenSomeAction()
    {
        // Execute
    }

    [Then(@"expected result")]
    public void ThenExpectedResult()
    {
        // Assert using AwesomeAssertions
        result.Should().NotBeNull();
    }
}
```

## Backend API Testing

### HTTP Client from Aspire

```csharp
[Binding]
public class ApiSteps(EnvironmentFixture fixture)
{
    private HttpClient? _httpClient;
    private HttpResponseMessage? _response;

    [Given(@"the application is running")]
    public void GivenTheApplicationIsRunning()
    {
        // Aspire orchestration already started via EnvironmentFixture
    }

    [When(@"I call the API endpoint")]
    public async Task WhenICallTheApiEndpoint()
    {
        _httpClient = fixture.App.CreateHttpClient("server");
        _response = await _httpClient.PostAsJsonAsync("/api/endpoint", request);
    }

    [Then(@"the response should be successful")]
    public void ThenTheResponseShouldBeSuccessful()
    {
        _response.Should().NotBeNull();
        _response!.IsSuccessStatusCode.Should().BeTrue();
    }
}
```

## Frontend Browser Testing

### Playwright Setup

```csharp
using Microsoft.Playwright;

[Binding]
public class UiSteps(EnvironmentFixture fixture)
{
    private IPage? _page;
    private IBrowser? _browser;
    private string? _clientUrl;

    [Given(@"I navigate to the page")]
    public async Task GivenINavigateToThePage()
    {
        // Get client URL from Aspire
        var clientHttpClient = fixture.App.CreateHttpClient("client");
        _clientUrl = clientHttpClient.BaseAddress!.ToString().TrimEnd('/');

        // Launch browser
        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true
        });

        _page = await _browser.NewPageAsync();
        await _page.GotoAsync($"{_clientUrl}/path");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [When(@"I interact with element")]
    public async Task WhenIInteractWithElement()
    {
        _page.Should().NotBeNull();
        
        // Fill input
        await _page!.FillAsync("input[name='field']", "value");
        
        // Click button
        await _page.ClickAsync("button:has-text('Submit')");
    }

    [Then(@"I should see result")]
    public async Task ThenIShouldSeeResult()
    {
        _page.Should().NotBeNull();
        
        // Wait for element
        var element = await _page!.WaitForSelectorAsync(".result");
        element.Should().NotBeNull();
        
        // Verify URL
        _page.Url.Should().Contain("/success");
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_page != null)
        {
            await _page.CloseAsync();
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
        }
    }
}
```

## Playwright Selectors

### Common Patterns

```csharp
// By attribute
await _page.FillAsync("input[formcontrolname='username']", "value");

// By text content
await _page.ClickAsync("button:has-text('Login')");

// By role
await _page.ClickAsync("role=button[name='Submit']");

// By test ID
await _page.ClickAsync("[data-testid='submit-btn']");

// CSS selector
await _page.QuerySelectorAsync(".mat-error");

// Angular component
await _page.QuerySelectorAsync("app-user-button");
```

### Waiting Strategies

```csharp
// Wait for navigation
await _page.WaitForURLAsync(url, new() { Timeout = 5000 });

// Wait for network idle
await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

// Wait for element
await _page.WaitForSelectorAsync("selector", new() { Timeout = 5000 });

// Manual delay (use sparingly)
await Task.Delay(1000);
```

## Assertions

Use AwesomeAssertions for all assertions:

```csharp
// Null checks
response.Should().NotBeNull();
element.Should().NotBeNull();

// Boolean
isSuccess.Should().BeTrue();
isDisabled.Should().BeTrue();

// String
message.Should().Be("Expected value");
url.Should().Contain("/path");

// HTTP
response!.IsSuccessStatusCode.Should().BeTrue();
response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
```

## Environment Fixture

The `EnvironmentFixture` is an assembly-level fixture that:
- Starts Aspire distributed application once for all tests
- Provides access to orchestrated services via `fixture.App`
- Creates HTTP clients for server and client endpoints
- Automatically disposed after all tests complete

**Do not modify the fixture** unless changing the Aspire orchestration setup.

## Running Tests

### Prerequisites

1. Install Playwright browsers:
   ```powershell
   pwsh tests/Central.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install
   ```

### Execute Tests

```powershell
# All acceptance tests
dotnet test tests/Central.AcceptanceTests/

# Specific feature
dotnet test tests/Central.AcceptanceTests/ --filter "FullyQualifiedName~Login"

# With detailed output
dotnet test tests/Central.AcceptanceTests/ --logger "console;verbosity=detailed"
```

## Best Practices

### DO

✅ Write scenarios in business language (Gherkin)
✅ Keep scenarios focused and independent
✅ Use AwesomeAssertions for all assertions
✅ Clean up resources in `[AfterScenario]`
✅ Use meaningful step names matching business language
✅ Reuse step definitions across features when appropriate
✅ Use headless browsers for CI/CD pipelines

### DON'T

❌ Don't access database directly (use API endpoints)
❌ Don't hard-code URLs (use fixture.App endpoints)
❌ Don't use Thread.Sleep (use Playwright waits)
❌ Don't mix backend and frontend testing in same steps
❌ Don't create dependencies between scenarios
❌ Don't use brittle selectors (prefer semantic/role-based)
❌ Don't forget to dispose browser resources

## Debugging

### Run with Headed Browser

```csharp
_browser = await playwright.Chromium.LaunchAsync(new()
{
    Headless = false,
    SlowMo = 500  // Slow down actions
});
```

### Screenshots on Failure

```csharp
[AfterScenario]
public async Task AfterScenario(ScenarioContext scenarioContext)
{
    if (scenarioContext.TestError != null && _page != null)
    {
        await _page.ScreenshotAsync(new()
        {
            Path = $"screenshot-{scenarioContext.ScenarioInfo.Title}.png"
        });
    }
    
    // ... cleanup
}
```

## CI/CD Considerations

- Tests run with headless browsers by default
- Aspire orchestrates all services (server, client, database)
- No manual service startup required
- Playwright browsers must be installed in CI environment
- Consider timeout configurations for slower environments

## Examples

See existing tests:
- `Features/SayHello.feature` - Backend API testing
- `Features/Login.feature` - Frontend browser testing with Playwright
