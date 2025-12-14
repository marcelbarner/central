using Central.Domain.Users;
using Central.Infrastructure;
using Central.Server.Infrastructure;

var bld = WebApplication.CreateBuilder(args);

// Add Infrastructure layer (DbContext, Identity stores)
var connectionString = bld.Configuration.GetConnectionString("centraldb")
    ?? throw new InvalidOperationException("Connection string 'centraldb' not found.");
bld.Services.AddInfrastructure(connectionString);

// Add SignInManager for authentication operations
bld.Services.AddScoped<Microsoft.AspNetCore.Identity.SignInManager<User>>();

// Configure cookie authentication with Identity
bld.Services.AddAuthentication(Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme)
    .AddCookie(Microsoft.AspNetCore.Identity.IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.Name = "Central.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
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

bld.Services
   .AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All)
   .SwaggerDocument();

// Add default Aspire health checks
bld.Services.AddHealthChecks();

var app = bld.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(
       c =>
       {
           c.Errors.UseProblemDetails();
       })
   .UseSwaggerGen();

// Map default Aspire health endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/alive");

app.Run();