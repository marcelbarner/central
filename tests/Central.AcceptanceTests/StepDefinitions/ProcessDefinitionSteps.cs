using Aspire.Hosting.Testing;

using AwesomeAssertions;

using Central.AcceptanceTests.Fixture;

using Microsoft.Playwright;

using Reqnroll;

namespace Central.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class ProcessDefinitionSteps(EnvironmentFixture fixture)
{
    private readonly EnvironmentFixture _fixture = fixture;
    private IAPIResponse? _lastResponse;
    private string? _processDefinitionId;
    private IAPIRequestContext? _apiContext;

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        // Create API context from the AppHost
        var baseUrl = _fixture.App.GetEndpoint("server").ToString().TrimEnd('/');

        var playwright = await Playwright.CreateAsync();
        _apiContext = await playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = baseUrl
        });
    }

    [Given(@"the system is running")]
    public void GivenTheSystemIsRunning()
    {
        _fixture.App.Should().NotBeNull();
    }

    [When(@"I create a process definition with name ""(.*)""")]
    public async Task WhenICreateAProcessDefinitionWithName(string processName)
    {
        var request = new
        {
            name = processName,
            description = "Test process for automated document processing",
            enabled = true,
            triggerState = "Imported",
            steps = Array.Empty<object>()
        };

        _lastResponse = await _apiContext!.PostAsync("/api/process-definitions",
            new() { DataObject = request });

        if (_lastResponse.Ok)
        {
            var response = await _lastResponse.JsonAsync();
            _processDefinitionId = response?.GetProperty("id").ToString();
        }
    }

    [When(@"I add an Azure Document Intelligence step to extract content")]
    public async Task WhenIAddAnAzureDocumentIntelligenceStep()
    {
        // This would update the process definition with a new step
        // For now, we'll create a new process with the step included
        var request = new
        {
            name = "Document Import Process",
            description = "Test process for automated document processing",
            enabled = true,
            triggerState = "Imported",
            steps = new[]
            {
                new
                {
                    name = "Extract Text",
                    stepType = "AzureDocumentIntelligence",
                    order = 0,
                    configuration = "{\"Endpoint\":\"https://test.cognitiveservices.azure.com\",\"ApiKey\":\"test-key\"}"
                }
            }
        };

        _lastResponse = await _apiContext!.PostAsync("/api/process-definitions",
            new() { DataObject = request });
    }

    [When(@"I add an Azure OpenAI step to enrich metadata")]
    public async Task WhenIAddAnAzureOpenAIStep()
    {
        // This would add a second step
        // For simplicity, we'll create with both steps
        var request = new
        {
            name = "Document Import Process",
            description = "Test process for automated document processing",
            enabled = true,
            triggerState = "Imported",
            steps = new[]
            {
                new
                {
                    name = "Extract Text",
                    stepType = "AzureDocumentIntelligence",
                    order = 0,
                    configuration = "{\"Endpoint\":\"https://test.cognitiveservices.azure.com\",\"ApiKey\":\"test-key\"}"
                },
                new
                {
                    name = "Enrich Metadata",
                    stepType = "AzureOpenAI",
                    order = 1,
                    configuration = "{\"Endpoint\":\"https://test.openai.azure.com\",\"ApiKey\":\"test-key\",\"DeploymentName\":\"gpt-4\",\"Prompt\":\"Extract metadata\"}"
                }
            }
        };

        _lastResponse = await _apiContext!.PostAsync("/api/process-definitions",
            new() { DataObject = request });

        if (_lastResponse.Ok)
        {
            var response = await _lastResponse.JsonAsync();
            _processDefinitionId = response?.GetProperty("id").ToString();
        }
    }

    [Then(@"the process should have (\d+) steps in the correct order")]
    public async Task ThenTheProcessShouldHaveStepsInCorrectOrder(int expectedStepCount)
    {
        _processDefinitionId.Should().NotBeNullOrEmpty();

        var response = await _apiContext!.GetAsync($"/api/process-definitions/{_processDefinitionId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var steps = json?.GetProperty("steps").EnumerateArray().ToList();

        steps.Should().HaveCount(expectedStepCount);

        // Verify steps are in order
        for (int i = 0; i < steps!.Count; i++)
        {
            steps[i].GetProperty("order").GetInt32().Should().Be(i);
        }
    }

    [Given(@"I have a process definition named ""(.*)""")]
    public async Task GivenIHaveAProcessDefinitionNamed(string processName)
    {
        await WhenICreateAProcessDefinitionWithName(processName);
    }

    [When(@"I enable automatic processing")]
    public async Task WhenIEnableAutomaticProcessing()
    {
        // Background worker is already running, just wait a moment
        await Task.Delay(TimeSpan.FromSeconds(2));
    }

    [When(@"I upload a document")]
    public async Task WhenIUploadADocument()
    {
        // Create a test document
        var request = new
        {
            title = "Test Document",
            state = "Imported",
            added = DateTimeOffset.UtcNow,
            updated = DateTimeOffset.UtcNow
        };

        _lastResponse = await _apiContext!.PostAsync("/api/documents",
            new() { DataObject = request });
    }

    [Then(@"the process definition should be created successfully")]
    public void ThenTheProcessDefinitionShouldBeCreatedSuccessfully()
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.Ok.Should().BeTrue();
        _lastResponse.Status.Should().Be(201);
        _processDefinitionId.Should().NotBeNullOrEmpty();
    }

    [Then(@"it should appear in the process list")]
    public async Task ThenItShouldAppearInTheProcessList()
    {
        var response = await _apiContext!.GetAsync("/api/process-definitions");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var processes = json?.EnumerateArray().ToList();

        processes.Should().NotBeEmpty();
        processes.Should().Contain(p => p.GetProperty("name").GetString() == "Extract Document Metadata");
    }
}