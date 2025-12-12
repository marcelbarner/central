using Aspire.Hosting.Testing;

using AwesomeAssertions;

using Central.AcceptanceTests.Fixture;
using Central.AcceptanceTests.PageObjects;

using Microsoft.Playwright;

using Reqnroll;

namespace Central.AcceptanceTests.StepDefinitions;

[Binding]
public class LoginSteps(EnvironmentFixture fixture)
{
    private IPage? _page;
    private string? _clientUrl;
    private LoginPage? _loginPage;
    private HomePage? _homePage;

    [Given(@"I navigate to the login page")]
    public async Task GivenINavigateToTheLoginPage()
    {
        // Get client endpoint URL
        _clientUrl = fixture.App.GetEndpoint("client").ToString().TrimEnd('/');

        // Create a new page from the shared browser
        _page = await fixture.Browser.NewPageAsync();
        
        // Initialize page objects
        _loginPage = new LoginPage(_page, _clientUrl);
        _homePage = new HomePage(_page, _clientUrl);
        
        await _loginPage.NavigateAsync();
    }

    [When(@"I enter username ""(.*)"" and password ""(.*)""")]
    public async Task WhenIEnterUsernameAndPassword(string username, string password)
    {
        _loginPage.Should().NotBeNull();
        await _loginPage!.FillCredentialsAsync(username, password);
    }

    [When(@"I click the login button")]
    public async Task WhenIClickTheLoginButton()
    {
        _loginPage.Should().NotBeNull();
        await _loginPage!.ClickLoginAsync();
        
        // Wait for navigation or error
        await Task.Delay(1000);
    }

    [Then(@"I should be redirected to the home page")]
    public async Task ThenIShouldBeRedirectedToTheHomePage()
    {
        _homePage.Should().NotBeNull();
        await _homePage!.WaitForPageLoadAsync();
        _homePage.GetCurrentUrl().Should().Be($"{_clientUrl}/dashboard");
    }

    [Then(@"I should see the user menu")]
    public async Task ThenIShouldSeeTheUserMenu()
    {
        _homePage.Should().NotBeNull();
        var isVisible = await _homePage!.IsUserButtonVisibleAsync();
        isVisible.Should().BeTrue();
    }

    [Then(@"I should see an error message")]
    public async Task ThenIShouldSeeAnErrorMessage()
    {
        _loginPage.Should().NotBeNull();
        var hasError = await _loginPage!.HasErrorMessageAsync();
        hasError.Should().BeTrue();
    }

    [Then(@"I should remain on the login page")]
    public void ThenIShouldRemainOnTheLoginPage()
    {
        _loginPage.Should().NotBeNull();
        _loginPage!.GetCurrentUrl().Should().Contain("/auth/login");
    }

    [Then(@"the login button should be disabled")]
    public async Task ThenTheLoginButtonShouldBeDisabled()
    {
        _loginPage.Should().NotBeNull();
        var isDisabled = await _loginPage!.IsLoginButtonDisabledAsync();
        isDisabled.Should().BeTrue();
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_page != null)
        {
            await _page.CloseAsync();
        }
    }
}
