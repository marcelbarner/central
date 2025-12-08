using Central.Domain.Authentication;
using Central.Infrastructure.Authentication;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var bld = WebApplication.CreateBuilder(args);

// Configure database (PostgreSQL)
// Aspire provides connection string as "centraldb", fallback to "DefaultConnection"
var connectionString = bld.Configuration.GetConnectionString("centraldb")
    ?? bld.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

bld.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure ASP.NET Core Identity
bld.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        // User settings
        options.User.RequireUniqueEmail = true;

        // Sign-in settings
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure cookie authentication
bld.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".Central.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

bld.Services
   .AddAuthorization()
   .AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All)
   .SwaggerDocument();

// Add default Aspire health checks
bld.Services.AddHealthChecks()
   .AddDbContextCheck<ApplicationDbContext>();

var app = bld.Build();

// Apply database migrations
await ApplyMigrations(app.Services);

// Seed test user
await SeedTestUser(app.Services);

app.UseAuthentication()
   .UseAuthorization()
   .UseFastEndpoints(
       c =>
       {
           c.Errors.UseProblemDetails();
       })
   .UseSwaggerGen();

// Map default Aspire health endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/alive");

app.Run();

static async Task ApplyMigrations(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

static async Task SeedTestUser(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var existingUser = await userManager.FindByNameAsync("testuser");
    if (existingUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, "Test123!");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}