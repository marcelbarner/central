using Central.AcceptanceTests.Fixture;

using Reqnroll;

namespace Central.AcceptanceTests.StepDefinitions;

[Binding]
public class EnvironmentSteps(EnvironmentFixture fixture)
{
    [Given("the application is running")]
    public async Task GivenTheApplicationIsRunning()
    {
        await fixture.InitializeAsync();
    }
}