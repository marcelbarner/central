using Microsoft.Playwright;

namespace Central.AcceptanceTests.PageObjects;

public class LoginPage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Locators
    private const string UsernameInput = "input[formcontrolname='username']";
    private const string PasswordInput = "input[formcontrolname='password']";
    private const string RememberMeCheckbox = "input[formcontrolname='rememberMe']";
    private const string LoginButton = "button:has-text('login')";
    private const string ErrorMessage = "mat-error";

    public LoginPage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    // Navigation
    public async Task NavigateAsync()
    {
        await _page.GotoAsync($"{_baseUrl}/auth/login");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    // Actions
    public async Task FillUsernameAsync(string username)
        => await _page.FillAsync(UsernameInput, username);

    public async Task FillPasswordAsync(string password)
        => await _page.FillAsync(PasswordInput, password);

    public async Task FillCredentialsAsync(string username, string password)
    {
        await FillUsernameAsync(username);
        await FillPasswordAsync(password);
    }

    public async Task CheckRememberMeAsync()
        => await _page.CheckAsync(RememberMeCheckbox);

    public async Task ClickLoginAsync()
        => await _page.ClickAsync(LoginButton);

    public async Task LoginAsync(string username, string password, bool rememberMe = false)
    {
        await FillCredentialsAsync(username, password);
        if (rememberMe)
        {
            await CheckRememberMeAsync();
        }
        await ClickLoginAsync();
    }

    // Queries
    public async Task<bool> IsLoginButtonDisabledAsync()
    {
        var button = await _page.QuerySelectorAsync(LoginButton);
        return button != null && await button.IsDisabledAsync();
    }

    public async Task<bool> HasErrorMessageAsync()
    {
        var error = await _page.QuerySelectorAsync(ErrorMessage);
        return error != null;
    }

    public async Task<string?> GetErrorMessageTextAsync()
    {
        var error = await _page.QuerySelectorAsync(ErrorMessage);
        return error != null ? await error.TextContentAsync() : null;
    }

    public string GetCurrentUrl() => _page.Url;

    public async Task WaitForNavigationAsync(string expectedPath)
    {
        await _page.WaitForURLAsync($"{_baseUrl}{expectedPath}", new() { Timeout = 5000 });
    }
}
