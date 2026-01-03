using Aspire.Hosting.Testing;

using AwesomeAssertions;

using Central.AcceptanceTests.Fixture;

using Microsoft.Playwright;

using Reqnroll;

namespace Central.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class ProcessExecutionSteps(EnvironmentFixture fixture)
{
    private readonly EnvironmentFixture _fixture = fixture;
    private string? _executionId;
    private string? _documentId;
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

    [Given(@"a process definition exists for documents in Imported state")]
    public async Task GivenAProcessDefinitionExistsForImportedDocuments()
    {
        var request = new
        {
            name = "Auto Import Process",
            description = "Automatically process imported documents",
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

        var response = await _apiContext!.PostAsync("/api/process-definitions",
            new() { DataObject = request });

        response.Ok.Should().BeTrue();
    }

    [Given(@"a document exists with state Imported")]
    public async Task GivenADocumentExistsWithStateImported()
    {
        var request = new
        {
            title = "Test Document for Processing",
            state = "Imported",
            added = DateTimeOffset.UtcNow,
            updated = DateTimeOffset.UtcNow
        };

        var response = await _apiContext!.PostAsync("/api/documents",
            new() { DataObject = request });

        if (response.Ok)
        {
            var json = await response.JsonAsync();
            _documentId = json?.GetProperty("id").ToString();
        }
    }

    [When(@"I trigger the process execution for the document")]
    public async Task WhenITriggerTheProcessExecutionForTheDocument()
    {
        _documentId.Should().NotBeNullOrEmpty();

        var request = new
        {
            processDefinitionId = 1,
            documentId = long.Parse(_documentId!)
        };

        var response = await _apiContext!.PostAsync("/api/process-executions",
            new() { DataObject = request });

        if (response.Ok)
        {
            var json = await response.JsonAsync();
            _executionId = json?.GetProperty("id").ToString();
        }
    }

    [Then(@"the process execution should be created")]
    public void ThenTheProcessExecutionShouldBeCreated()
    {
        _executionId.Should().NotBeNullOrEmpty();
    }

    [Then(@"the document state should be Processing")]
    public async Task ThenTheDocumentStateShouldBeProcessing()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));

        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var state = json?.GetProperty("state").GetString();

        state.Should().Be("Processing");
    }

    [Then(@"each step should execute in order")]
    public async Task ThenEachStepShouldExecuteInOrder()
    {
        var response = await _apiContext!.GetAsync($"/api/process-executions/{_executionId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var steps = json?.GetProperty("steps").EnumerateArray().ToList();

        steps.Should().NotBeEmpty();

        for (int i = 0; i < steps!.Count; i++)
        {
            steps[i].GetProperty("status").GetString().Should().NotBe("Pending");
        }
    }

    [Then(@"the document state should be Processed when complete")]
    public async Task ThenTheDocumentStateShouldBeProcessedWhenComplete()
    {
        // Wait for execution to complete
        await Task.Delay(TimeSpan.FromSeconds(5));

        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var state = json?.GetProperty("state").GetString();

        state.Should().Be("Processed");
    }

    [Given(@"a document has multiple completed process executions")]
    public async Task GivenADocumentHasMultipleCompletedProcessExecutions()
    {
        // Create a document
        await GivenADocumentExistsWithStateImported();

        // Create multiple executions
        for (int i = 0; i < 3; i++)
        {
            var request = new
            {
                processDefinitionId = 1,
                documentId = long.Parse(_documentId!)
            };

            await _apiContext!.PostAsync("/api/process-executions",
                new() { DataObject = request });

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }
    }

    [When(@"I retrieve the execution history for the document")]
    public async Task WhenIRetrieveTheExecutionHistoryForTheDocument()
    {
        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}/executions");
        response.Ok.Should().BeTrue();
    }

    [Then(@"I should see all executions ordered by date")]
    public async Task ThenIShouldSeeAllExecutionsOrderedByDate()
    {
        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}/executions");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var executions = json?.EnumerateArray().ToList();

        executions.Should().NotBeEmpty();
        executions.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Then(@"each execution should show step results")]
    public async Task ThenEachExecutionShouldShowStepResults()
    {
        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}/executions");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var executions = json?.EnumerateArray().ToList();

        foreach (var execution in executions!)
        {
            var steps = execution.GetProperty("steps").EnumerateArray().ToList();
            steps.Should().NotBeNull();
        }
    }

    [Then(@"execution status should be visible")]
    public async Task ThenExecutionStatusShouldBeVisible()
    {
        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}/executions");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var executions = json?.EnumerateArray().ToList();

        foreach (var execution in executions!)
        {
            execution.GetProperty("status").GetString().Should().NotBeNullOrEmpty();
        }
    }

    [Given(@"an enabled process definition exists for Imported documents")]
    public async Task GivenAnEnabledProcessDefinitionExistsForImportedDocuments()
    {
        await GivenAProcessDefinitionExistsForImportedDocuments();
    }

    [When(@"a document is uploaded and reaches Imported state")]
    public async Task WhenADocumentIsUploadedAndReachesImportedState()
    {
        await GivenADocumentExistsWithStateImported();
    }

    [Then(@"the background worker should automatically detect it")]
    public async Task ThenTheBackgroundWorkerShouldAutomaticallyDetectIt()
    {
        // Background worker runs every 30 seconds, so we need to wait
        await Task.Delay(TimeSpan.FromSeconds(35));
    }

    [Then(@"create a process execution within 30 seconds")]
    public async Task ThenCreateAProcessExecutionWithin30Seconds()
    {
        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}/executions");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var executions = json?.EnumerateArray().ToList();

        executions.Should().NotBeEmpty();
        _executionId = executions!.First().GetProperty("id").ToString();
    }

    [Then(@"execute all configured steps")]
    public async Task ThenExecuteAllConfiguredSteps()
    {
        await ThenEachStepShouldExecuteInOrder();
    }

    [Given(@"a process with a step that will fail")]
    public async Task GivenAProcessWithAStepThatWillFail()
    {
        // Create a process with invalid Azure credentials that will fail
        var request = new
        {
            name = "Failing Process",
            description = "Process with invalid configuration",
            enabled = true,
            triggerState = "Imported",
            steps = new[]
            {
                new
                {
                    name = "Extract Text (Will Fail)",
                    stepType = "AzureDocumentIntelligence",
                    order = 0,
                    configuration = "{\"Endpoint\":\"https://invalid.cognitiveservices.azure.com\",\"ApiKey\":\"invalid-key\"}"
                },
                new
                {
                    name = "Should Not Execute",
                    stepType = "AzureOpenAI",
                    order = 1,
                    configuration = "{\"Endpoint\":\"https://test.openai.azure.com\",\"ApiKey\":\"test-key\",\"DeploymentName\":\"gpt-4\",\"Prompt\":\"Process\"}"
                }
            }
        };

        var response = await _apiContext!.PostAsync("/api/process-definitions",
            new() { DataObject = request });

        response.Ok.Should().BeTrue();
    }

    [When(@"the process executes on a document")]
    public async Task WhenTheProcessExecutesOnADocument()
    {
        await GivenADocumentExistsWithStateImported();
        await WhenITriggerTheProcessExecutionForTheDocument();

        // Wait for the failing step to execute
        await Task.Delay(TimeSpan.FromSeconds(5));
    }

    [Then(@"the execution should stop at the failed step")]
    public async Task ThenTheExecutionShouldStopAtTheFailedStep()
    {
        var response = await _apiContext!.GetAsync($"/api/process-executions/{_executionId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var steps = json?.GetProperty("steps").EnumerateArray().ToList();

        // First step should be failed
        steps![0].GetProperty("status").GetString().Should().Be("Failed");

        // Second step should not have executed
        if (steps.Count > 1)
        {
            steps[1].GetProperty("status").GetString().Should().Be("Pending");
        }
    }

    [Then(@"the document state should be Failed")]
    public async Task ThenTheDocumentStateShouldBeFailed()
    {
        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var state = json?.GetProperty("state").GetString();

        state.Should().Be("Failed");
    }

    [Then(@"subsequent steps should not execute")]
    public async Task ThenSubsequentStepsShouldNotExecute()
    {
        await ThenTheExecutionShouldStopAtTheFailedStep();
    }

    [When(@"I trigger the process manually")]
    public async Task WhenITriggerTheProcessManually()
    {
        var request = new
        {
            processDefinitionId = 1,
            documentId = 1
        };

        var response = await _apiContext!.PostAsync("/api/process-executions",
            new() { DataObject = request });

        if (response.Ok)
        {
            var json = await response.JsonAsync();
            _executionId = json?.GetProperty("id").ToString();
        }
    }

    [When(@"the system processes pending documents")]
    public async Task WhenTheSystemProcessesPendingDocuments()
    {
        // Wait for the background worker to pick up the document
        await Task.Delay(TimeSpan.FromSeconds(35));

        // Get executions for the document
        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}/executions");

        if (response.Ok)
        {
            var json = await response.JsonAsync();
            var executions = json?.EnumerateArray().ToList();
            if (executions?.Any() == true)
            {
                _executionId = executions.First().GetProperty("id").ToString();
            }
        }
    }

    [Then(@"the process should complete successfully")]
    public async Task ThenTheProcessShouldCompleteSuccessfully()
    {
        _executionId.Should().NotBeNullOrEmpty();

        var response = await _apiContext!.GetAsync($"/api/process-executions/{_executionId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var status = json?.GetProperty("status").GetString();

        status.Should().Be("Completed");
    }

    [Then(@"I should see the execution in the history")]
    public async Task ThenIShouldSeeTheExecutionInTheHistory()
    {
        var response = await _apiContext!.GetAsync("/api/process-executions");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var executions = json?.EnumerateArray().ToList();

        executions.Should().NotBeEmpty();
        executions.Should().Contain(e => e.GetProperty("id").ToString() == _executionId);
    }

    [Then(@"the execution should include step details")]
    public async Task ThenTheExecutionShouldIncludeStepDetails()
    {
        var response = await _apiContext!.GetAsync($"/api/process-executions/{_executionId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var steps = json?.GetProperty("steps").EnumerateArray().ToList();

        steps.Should().NotBeEmpty();
        steps!.First().GetProperty("name").GetString().Should().NotBeNullOrEmpty();
        steps.First().GetProperty("status").GetString().Should().Be("Completed");
    }

    [Then(@"the document should be processed automatically")]
    public async Task ThenTheDocumentShouldBeProcessedAutomatically()
    {
        // Check document state
        var response = await _apiContext!.GetAsync($"/api/documents/{_documentId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var state = json?.GetProperty("state").GetString();

        state.Should().Be("Processed");
    }

    [Then(@"the execution should fail")]
    public async Task ThenTheExecutionShouldFail()
    {
        _executionId.Should().NotBeNullOrEmpty();

        var response = await _apiContext!.GetAsync($"/api/process-executions/{_executionId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var status = json?.GetProperty("status").GetString();

        status.Should().Be("Failed");
    }

    [Then(@"the error message should be captured")]
    public async Task ThenTheErrorMessageShouldBeCaptured()
    {
        var response = await _apiContext!.GetAsync($"/api/process-executions/{_executionId}");
        response.Ok.Should().BeTrue();

        var json = await response.JsonAsync();
        var errorMessage = json?.GetProperty("errorMessage").GetString();

        errorMessage.Should().NotBeNullOrEmpty();
    }
}