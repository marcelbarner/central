using System.Net;
using System.Net.Http.Json;

using Aspire.Hosting.Testing;

using Central.AcceptanceTests.Fixture;

using Reqnroll;

namespace Central.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class SayHelloSteps(EnvironmentFixture fixture)
{
    private HttpClient? _httpClient;
    private HttpResponseMessage? _response;

    [When(@"I send a greeting with first name ""(.*)"" and last name ""(.*)""")]
    public async Task WhenISendAGreetingWithFirstNameAndLastName(string firstName, string lastName)
    {
        _httpClient = fixture.App.CreateHttpClient("server");
        var request = new
        {
            FirstName = firstName,
            LastName = lastName
        };

        _response = await _httpClient!.PostAsJsonAsync("/api/hello", request);
    }

    [Then(@"the response should be successful")]
    public void ThenTheResponseShouldBeSuccessful()
    {
        Assert.NotNull(_response);
        Assert.True(_response.IsSuccessStatusCode, $"Expected successful response but got {_response.StatusCode}");
    }

    [Then(@"the greeting message should be ""(.*)""")]
    public async Task ThenTheGreetingMessageShouldBe(string expectedMessage)
    {
        Assert.NotNull(_response);
        
        var content = await _response.Content.ReadFromJsonAsync<GreetingResponse>();
        Assert.NotNull(content);
        Assert.Equal(expectedMessage, content.Message);
    }

    [Then(@"the response should indicate validation error")]
    public void ThenTheResponseShouldIndicateValidationError()
    {
        Assert.NotNull(_response);
        Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
    }

    private sealed class GreetingResponse
    {
        public required string Message { get; set; }
    }
}
