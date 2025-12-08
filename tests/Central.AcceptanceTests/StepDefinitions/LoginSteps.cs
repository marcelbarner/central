using Aspire.Hosting.Testing;

using AwesomeAssertions;

using Central.AcceptanceTests.Fixture;

using Microsoft.Playwright;

using Reqnroll;

namespace Central.AcceptanceTests.StepDefinitions;

[Binding]
public class LoginSteps(EnvironmentFixture fixture)
{
    private IPage? _page;
    private IBrowser? _browser;
    private string? _clientUrl;

    [Given(@"I navigate to the login page")]
    public async Task GivenINavigateToTheLoginPage()
    {
        // Get client HTTP client and extract base URL
        var clientHttpClient = fixture.App.CreateHttpClient("client");
        _clientUrl = clientHttpClient.BaseAddress!.ToString().TrimEnd('/');

        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true
        });

        _page = await _browser.NewPageAsync();
        await _page.GotoAsync($"{_clientUrl}/auth/login");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [When(@"I enter username ""(.*)"" and password ""(.*)""")]
    public async Task WhenIEnterUsernameAndPassword(string username, string password)
    {
        _page.Should().NotBeNull();

        // Find and fill username field
        await _page!.FillAsync("input[formcontrolname='username']", username);

        // Find and fill password field
        await _page.FillAsync("input[formcontrolname='password']", password);
    }

    [When(@"I click the login button")]
    public async Task WhenIClickTheLoginButton()
    {
        _page.Should().NotBeNull();

        // Click the login button
        await _page!.ClickAsync("button:has-text('login')");

        // Wait for navigation or error
        await Task.Delay(1000);
    }

    [Then(@"I should be redirected to the home page")]
    public async Task ThenIShouldBeRedirectedToTheHomePage()
    {
        _page.Should().NotBeNull();

        await _page!.WaitForURLAsync($"{_clientUrl}/", new() { Timeout = 5000 });
        _page.Url.Should().Be($"{_clientUrl}/");
    }

    [Then(@"I should see the user menu")]
    public async Task ThenIShouldSeeTheUserMenu()
    {
        _page.Should().NotBeNull();

        var userButton = await _page!.QuerySelectorAsync("app-user-button");
        userButton.Should().NotBeNull();
    }

    [Then(@"I should see an error message")]
    public async Task ThenIShouldSeeAnErrorMessage()
    {
        _page.Should().NotBeNull();

        // Wait for error message to appear
        var errorElement = await _page!.WaitForSelectorAsync("mat-error", new() { Timeout = 5000 });
        errorElement.Should().NotBeNull();
    }

    [Then(@"I should remain on the login page")]
    public async Task ThenIShouldRemainOnTheLoginPage()
    {
        _page.Should().NotBeNull();

        _page!.Url.Should().Contain("/auth/login");
    }

    [Then(@"the login button should be disabled")]
    public async Task ThenTheLoginButtonShouldBeDisabled()
    {
        _page.Should().NotBeNull();

        var loginButton = await _page!.QuerySelectorAsync("button:has-text('login')");
        loginButton.Should().NotBeNull();

        var isDisabled = await loginButton!.IsDisabledAsync();
        isDisabled.Should().BeTrue();
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
