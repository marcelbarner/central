using System.Net;
using System.Net.Http.Json;

using Aspire.Hosting.Testing;
using AwesomeAssertions;

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
        _response.Should().NotBeNull();
        _response!.IsSuccessStatusCode.Should().BeTrue($"Expected successful response but got {_response.StatusCode}");
    }

    [Then(@"the greeting message should be ""(.*)""")]
    public async Task ThenTheGreetingMessageShouldBe(string expectedMessage)
    {
        _response.Should().NotBeNull();
        var content = await _response!.Content.ReadFromJsonAsync<GreetingResponse>();
        content.Should().NotBeNull();
        content!.Message.Should().Be(expectedMessage);
    }

    [Then(@"the response should indicate validation error")]
    public void ThenTheResponseShouldIndicateValidationError()
    {
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed class GreetingResponse
    {
        public required string Message { get; set; }
    }
}
