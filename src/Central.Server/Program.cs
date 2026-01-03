using Central.Domain.Users;
using Central.Domain.Users.Ports;
using Central.Infrastructure;
using Central.Server.Infrastructure;
using Central.Server.Infrastructure.Services;

var bld = WebApplication.CreateBuilder(args);

// Add Infrastructure layer (DbContext, Identity stores)
var connectionString = bld.Configuration.GetConnectionString("centraldb")
    ?? throw new InvalidOperationException("Connection string 'centraldb' not found.");
bld.Services.AddInfrastructure(connectionString, bld.Configuration);

// Add SignInManager for authentication operations
bld.Services.AddScoped<Microsoft.AspNetCore.Identity.SignInManager<User>>();

// Add HTTP context accessor and current user service
bld.Services.AddHttpContextAccessor();
bld.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Configure cookie authentication with Identity
bld.Services.AddAuthentication(Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme)
    .AddCookie(Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.Name = "Central.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        // When isPersistent is true (RememberMe), cookie expires after this timespan
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });
bld.Services.AddAuthorization();

// Add background service for database initialization
bld.Services.AddHostedService<DatabaseInitializerService>();

// Add background service for process execution
bld.Services.AddHostedService<ProcessExecutionWorker>();

bld.Services
   .AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All)
   .SwaggerDocument();

// Add default Aspire health checks
bld.Services.AddHealthChecks();

var app = bld.Build();

// Serve static files from wwwroot (Angular build output)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(
       c =>
       {
           c.Errors.UseProblemDetails();
       })
   .UseSwaggerGen();

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/alive");

// Fallback to index.html for Angular routing
app.MapFallbackToFile("index.html");

app.Run();