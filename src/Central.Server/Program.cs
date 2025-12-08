var bld = WebApplication.CreateBuilder(args);

bld.Services
   .AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All)
   .SwaggerDocument();

// Add default Aspire health checks
bld.Services.AddHealthChecks();

var app = bld.Build();

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