
using Aspire.Hosting;
using Aspire.Hosting.Testing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

[assembly: AssemblyFixture(typeof(Central.AcceptanceTests.Fixture.EnvironmentFixture))]
namespace Central.AcceptanceTests.Fixture;

public sealed class EnvironmentFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    
    public DistributedApplication App { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;
    
    public async ValueTask DisposeAsync()
    {
        if (Browser != null)
        {
            await Browser.DisposeAsync();
        }
        await App.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Central_AppHost>();

        builder.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        App = await builder.BuildAsync()
            .WaitAsync(DefaultTimeout);
        await App.StartAsync()
            .WaitAsync(DefaultTimeout);
        
        // Initialize Playwright browser once for all tests
        var playwright = await Playwright.CreateAsync();
        Browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = true
        });
    }
}