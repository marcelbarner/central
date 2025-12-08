# API Design

## FastEndpoints

* **Organization**: Feature folders under `Features/`
* **Structure**: Each feature contains `Endpoint.cs`, `Request.cs`, `Response.cs`
* **Validation**: FluentValidation inline with request classes
* **Authentication**: Cookie, API token, and basic authentication
* **Documentation**: Swagger/OpenAPI auto-generated

## Endpoint Pattern

```csharp
sealed class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/resource");
        // Configure auth, policies, etc.
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        // 1. Map DTO to Domain
        // 2. Execute business logic
        // 3. Map result to Response
        await SendAsync(response);
    }
}
```
