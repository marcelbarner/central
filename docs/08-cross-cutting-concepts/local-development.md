# Local Development

## Aspire

* **Purpose**: Local development orchestration
* **Configuration**: Single-file host (`apphost.cs`)
* **Services**: Manages PostgreSQL, API server, and dependencies
* **Dashboard**: Visual monitoring of running services

## Development Workflow

1. Run Aspire host: `dotnet run --project apphost.cs`
2. Aspire starts all dependencies (PostgreSQL, etc.)
3. API launches with proper configuration
4. Dashboard available for monitoring
5. Frontend connects to local API
