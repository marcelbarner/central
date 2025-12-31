using System.Net.Http.Json;
using System.Text.Json;

using Aspire.Hosting.Testing;

using AwesomeAssertions;

using Central.AcceptanceTests.Fixture;

using Reqnroll;

namespace Central.AcceptanceTests.StepDefinitions;

[Binding]
public class ContractManagementSteps(EnvironmentFixture fixture)
{
    private string? _apiUrl;
    private HttpClient? _httpClient;
    private long? _createdContractId;
    private readonly List<long> _contractIds = [];
    private readonly Dictionary<string, long> _contractsByName = [];
    private readonly Dictionary<string, long> _correspondentsByName = [];
    private readonly Dictionary<string, long> _documentsByTitle = [];
    private HttpResponseMessage? _lastResponse;
    private string? _lastErrorMessage;

    [BeforeScenario]
    public void BeforeScenario()
    {
        _apiUrl = fixture.App.GetEndpoint("server").ToString().TrimEnd('/');
        _httpClient = new HttpClient { BaseAddress = new Uri(_apiUrl) };
    }

    [Given(@"a correspondent exists with name ""(.*)""")]
    public async Task GivenACorrespondentExistsWithName(string name)
    {
        _httpClient.Should().NotBeNull();

        var request = new
        {
            Name = name,
            Description = $"Test correspondent {name}"
        };

        var response = await _httpClient!.PostAsJsonAsync("/api/correspondents", request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var correspondent = JsonSerializer.Deserialize<JsonElement>(responseContent);
        var id = correspondent.GetProperty("id").GetInt64();
        _correspondentsByName[name] = id;
    }

    [Given(@"a contract exists with name ""(.*)""")]
    public async Task GivenAContractExistsWithName(string name)
    {
        await CreateContract(name, "Test contract", "Draft", null);
    }

    [Given(@"a contract exists with name ""(.*)"" and correspondent ""(.*)""")]
    public async Task GivenAContractExistsWithNameAndCorrespondent(string contractName, string correspondentName)
    {
        var correspondentId = _correspondentsByName[correspondentName];
        await CreateContract(contractName, "Test contract", "Active", correspondentId);
    }

    [Given(@"the following contracts exist:")]
    public async Task GivenTheFollowingContractsExist(Table table)
    {
        foreach (var row in table.Rows)
        {
            var name = row["Name"];
            var state = row["State"];
            await CreateContract(name, $"Test contract {name}", state, null);
        }
    }

    [Given(@"a document exists with title ""(.*)""")]
    public async Task GivenADocumentExistsWithTitle(string title)
    {
        _httpClient.Should().NotBeNull();

        var request = new
        {
            Title = title,
            DocumentDate = DateTimeOffset.UtcNow,
            Content = $"Content for {title}"
        };

        var response = await _httpClient!.PostAsJsonAsync("/api/documents", request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var document = JsonSerializer.Deserialize<JsonElement>(responseContent);
        var id = document.GetProperty("id").GetInt64();
        _documentsByTitle[title] = id;
    }

    [Given(@"a document exists with title ""(.*)"" without correspondent")]
    public async Task GivenADocumentExistsWithTitleWithoutCorrespondent(string title)
    {
        await GivenADocumentExistsWithTitle(title);
    }

    [Given(@"a document exists with title ""(.*)"" with correspondent ""(.*)""")]
    public async Task GivenADocumentExistsWithTitleWithCorrespondent(string title, string correspondentName)
    {
        _httpClient.Should().NotBeNull();

        var correspondentId = _correspondentsByName[correspondentName];

        var request = new
        {
            Title = title,
            DocumentDate = DateTimeOffset.UtcNow,
            Content = $"Content for {title}",
            CorrespondentId = correspondentId
        };

        var response = await _httpClient!.PostAsJsonAsync("/api/documents", request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var document = JsonSerializer.Deserialize<JsonElement>(responseContent);
        var id = document.GetProperty("id").GetInt64();
        _documentsByTitle[title] = id;
    }

    [Given(@"the document is assigned to the contract ""(.*)""")]
    public async Task GivenTheDocumentIsAssignedToTheContract(string contractName)
    {
        var contractId = _contractsByName[contractName];
        var documentId = _documentsByTitle.Values.Last();

        var request = new
        {
            DocumentId = documentId,
            SyncCorrespondent = false
        };

        var response = await _httpClient!.PostAsJsonAsync($"/api/contracts/{contractId}/assign-to-document", request);
        response.EnsureSuccessStatusCode();
    }

    [When(@"I create a contract with the following details:")]
    public async Task WhenICreateAContractWithTheFollowingDetails(Table table)
    {
        _httpClient.Should().NotBeNull();

        var row = table.Rows[0];
        var name = row["Name"];
        var description = row["Description"];
        var state = row["State"];
        long? correspondentId = null;

        if (row.ContainsKey("CorrespondentName"))
        {
            var correspondentName = row["CorrespondentName"];
            correspondentId = _correspondentsByName[correspondentName];
        }

        await CreateContract(name, description, state, correspondentId);
    }

    [When(@"I retrieve all contracts")]
    public async Task WhenIRetrieveAllContracts()
    {
        _httpClient.Should().NotBeNull();

        _lastResponse = await _httpClient!.GetAsync("/api/contracts");
        _lastResponse.EnsureSuccessStatusCode();
    }

    [When(@"I retrieve the contract by its ID")]
    public async Task WhenIRetrieveTheContractByItsId()
    {
        _httpClient.Should().NotBeNull();
        _createdContractId.Should().NotBeNull();

        _lastResponse = await _httpClient!.GetAsync($"/api/contracts/{_createdContractId}");
        _lastResponse.EnsureSuccessStatusCode();
    }

    [When(@"I update the contract with the following details:")]
    public async Task WhenIUpdateTheContractWithTheFollowingDetails(Table table)
    {
        _httpClient.Should().NotBeNull();
        _createdContractId.Should().NotBeNull();

        var row = table.Rows[0];
        var name = row["Name"];
        var description = row["Description"];
        var state = row["State"];

        var request = new
        {
            Id = _createdContractId,
            Name = name,
            Description = description,
            State = state
        };

        _lastResponse = await _httpClient!.PutAsJsonAsync($"/api/contracts/{_createdContractId}", request);
        _lastResponse.EnsureSuccessStatusCode();
    }

    [When(@"I delete the contract")]
    public async Task WhenIDeleteTheContract()
    {
        _httpClient.Should().NotBeNull();
        _createdContractId.Should().NotBeNull();

        try
        {
            _lastResponse = await _httpClient!.DeleteAsync($"/api/contracts/{_createdContractId}");
        }
        catch (Exception ex)
        {
            _lastErrorMessage = ex.Message;
        }
    }

    [When(@"I assign the contract ""(.*)"" to the document ""(.*)""")]
    public async Task WhenIAssignTheContractToTheDocument(string contractName, string documentTitle)
    {
        var contractId = _contractsByName[contractName];
        var documentId = _documentsByTitle[documentTitle];

        var request = new
        {
            DocumentId = documentId,
            SyncCorrespondent = false
        };

        _lastResponse = await _httpClient!.PostAsJsonAsync($"/api/contracts/{contractId}/assign-to-document", request);
        _lastResponse.EnsureSuccessStatusCode();
    }

    [When(@"I assign the contract ""(.*)"" to the document ""(.*)"" with correspondent sync enabled")]
    public async Task WhenIAssignTheContractToTheDocumentWithCorrespondentSyncEnabled(string contractName, string documentTitle)
    {
        var contractId = _contractsByName[contractName];
        var documentId = _documentsByTitle[documentTitle];

        var request = new
        {
            DocumentId = documentId,
            SyncCorrespondent = true
        };

        _lastResponse = await _httpClient!.PostAsJsonAsync($"/api/contracts/{contractId}/assign-to-document", request);
        _lastResponse.EnsureSuccessStatusCode();
    }

    [When(@"I assign the contract ""(.*)"" to the document ""(.*)"" with correspondent sync disabled")]
    public async Task WhenIAssignTheContractToTheDocumentWithCorrespondentSyncDisabled(string contractName, string documentTitle)
    {
        var contractId = _contractsByName[contractName];
        var documentId = _documentsByTitle[documentTitle];

        var request = new
        {
            DocumentId = documentId,
            SyncCorrespondent = false
        };

        _lastResponse = await _httpClient!.PostAsJsonAsync($"/api/contracts/{contractId}/assign-to-document", request);
        _lastResponse.EnsureSuccessStatusCode();
    }

    [Then(@"the contract should be created successfully")]
    public void ThenTheContractShouldBeCreatedSuccessfully()
    {
        _createdContractId.Should().NotBeNull();
        _createdContractId.Should().BeGreaterThan(0);
    }

    [Then(@"the contract should be updated successfully")]
    public void ThenTheContractShouldBeUpdatedSuccessfully()
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.IsSuccessStatusCode.Should().BeTrue();
    }

    [Then(@"the contract should be deleted successfully")]
    public void ThenTheContractShouldBeDeletedSuccessfully()
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.IsSuccessStatusCode.Should().BeTrue();
    }

    [Then(@"the deletion should fail")]
    public void ThenTheDeletionShouldFail()
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.IsSuccessStatusCode.Should().BeFalse();
    }

    [Then(@"an error message should indicate documents are still associated")]
    public async Task ThenAnErrorMessageShouldIndicateDocumentsAreStillAssociated()
    {
        _lastResponse.Should().NotBeNull();
        var content = await _lastResponse!.Content.ReadAsStringAsync();
        content.Should().Contain("document");
    }

    [Then(@"the contract should have name ""(.*)""")]
    public async Task ThenTheContractShouldHaveName(string expectedName)
    {
        var contract = await GetCreatedContract();
        var name = contract.GetProperty("name").GetString();
        name.Should().Be(expectedName);
    }

    [Then(@"the contract should have state ""(.*)""")]
    public async Task ThenTheContractShouldHaveState(string expectedState)
    {
        var contract = await GetCreatedContract();
        var state = contract.GetProperty("state").GetString();
        state.Should().Be(expectedState);
    }

    [Then(@"the contract should have correspondent ""(.*)""")]
    public async Task ThenTheContractShouldHaveCorrespondent(string expectedCorrespondentName)
    {
        var contract = await GetCreatedContract();
        var correspondentId = contract.GetProperty("correspondentId").GetInt64();
        var expectedId = _correspondentsByName[expectedCorrespondentName];
        correspondentId.Should().Be(expectedId);
    }

    [Then(@"I should see (.*) contracts")]
    public async Task ThenIShouldSeeContracts(int expectedCount)
    {
        _lastResponse.Should().NotBeNull();
        var content = await _lastResponse!.Content.ReadAsStringAsync();
        var contracts = JsonSerializer.Deserialize<JsonElement>(content);
        contracts.GetArrayLength().Should().Be(expectedCount);
    }

    [Then(@"the contracts should include ""(.*)""")]
    public async Task ThenTheContractsShouldInclude(string contractName)
    {
        _lastResponse.Should().NotBeNull();
        var content = await _lastResponse!.Content.ReadAsStringAsync();
        var contracts = JsonSerializer.Deserialize<JsonElement>(content);
        var found = contracts.EnumerateArray().Any(c => c.GetProperty("name").GetString() == contractName);
        found.Should().BeTrue();
    }

    [Then(@"the contract details should be returned")]
    public void ThenTheContractDetailsShouldBeReturned()
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.IsSuccessStatusCode.Should().BeTrue();
    }

    [Then(@"the document should be associated with the contract")]
    public void ThenTheDocumentShouldBeAssociatedWithTheContract()
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.IsSuccessStatusCode.Should().BeTrue();
    }

    [Then(@"the document should have contract ""(.*)""")]
    public async Task ThenTheDocumentShouldHaveContract(string contractName)
    {
        var documentId = _documentsByTitle.Values.Last();
        var response = await _httpClient!.GetAsync($"/api/documents/{documentId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var document = JsonSerializer.Deserialize<JsonElement>(content);
        var contractId = document.GetProperty("contractId").GetInt64();
        var expectedId = _contractsByName[contractName];
        contractId.Should().Be(expectedId);
    }

    [Then(@"the document should have correspondent ""(.*)""")]
    public async Task ThenTheDocumentShouldHaveCorrespondent(string correspondentName)
    {
        var documentId = _documentsByTitle.Values.Last();
        var response = await _httpClient!.GetAsync($"/api/documents/{documentId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var document = JsonSerializer.Deserialize<JsonElement>(content);
        var correspondentId = document.GetProperty("correspondentId").GetInt64();
        var expectedId = _correspondentsByName[correspondentName];
        correspondentId.Should().Be(expectedId);
    }

    [Then(@"the document should still have correspondent ""(.*)""")]
    public async Task ThenTheDocumentShouldStillHaveCorrespondent(string correspondentName)
    {
        await ThenTheDocumentShouldHaveCorrespondent(correspondentName);
    }

    [Then(@"the document should be associated with the contract ""(.*)""")]
    public async Task ThenTheDocumentShouldBeAssociatedWithTheContract(string contractName)
    {
        await ThenTheDocumentShouldHaveContract(contractName);
    }

    [Then(@"the document should not be associated with the contract ""(.*)""")]
    public async Task ThenTheDocumentShouldNotBeAssociatedWithTheContract(string contractName)
    {
        var documentId = _documentsByTitle.Values.Last();
        var response = await _httpClient!.GetAsync($"/api/documents/{documentId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var document = JsonSerializer.Deserialize<JsonElement>(content);

        if (document.TryGetProperty("contractId", out var contractIdProperty) &&
            contractIdProperty.ValueKind != JsonValueKind.Null)
        {
            var contractId = contractIdProperty.GetInt64();
            var oldContractId = _contractsByName[contractName];
            contractId.Should().NotBe(oldContractId);
        }
    }

    private async Task CreateContract(string name, string description, string state, long? correspondentId)
    {
        _httpClient.Should().NotBeNull();

        var request = new
        {
            Name = name,
            Description = description,
            State = state,
            CorrespondentId = correspondentId
        };

        var response = await _httpClient!.PostAsJsonAsync("/api/contracts", request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var contract = JsonSerializer.Deserialize<JsonElement>(responseContent);
        var id = contract.GetProperty("id").GetInt64();
        _createdContractId = id;
        _contractIds.Add(id);
        _contractsByName[name] = id;
    }

    private async Task<JsonElement> GetCreatedContract()
    {
        _httpClient.Should().NotBeNull();
        _createdContractId.Should().NotBeNull();

        var response = await _httpClient!.GetAsync($"/api/contracts/{_createdContractId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(content);
    }
}