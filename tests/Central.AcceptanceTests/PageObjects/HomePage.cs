using Microsoft.Playwright;

namespace Central.AcceptanceTests.PageObjects;

public class HomePage
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    // Locators
    private const string UserButton = "app-user";
    private const string UserMenu = "[role='menu']";
    private const string LogoutButton = "button:has-text('Logout')";

    public HomePage(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    // Navigation
    public async Task NavigateAsync()
    {
        await _page.GotoAsync(_baseUrl);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    // Queries
    public async Task<bool> IsUserButtonVisibleAsync()
    {
        var userButton = await _page.QuerySelectorAsync(UserButton);
        return userButton != null && await userButton.IsVisibleAsync();
    }

    public string GetCurrentUrl() => _page.Url;

    public async Task WaitForPageLoadAsync()
    {
        await _page.WaitForURLAsync($"{_baseUrl}/dashboard", new() { Timeout = 5000 });
    }

    // Actions
    public async Task OpenUserMenuAsync()
    {
        await _page.ClickAsync(UserButton);
        await _page.WaitForSelectorAsync(UserMenu);
    }

    public async Task LogoutAsync()
    {
        await OpenUserMenuAsync();
        await _page.ClickAsync(LogoutButton);
    }
}