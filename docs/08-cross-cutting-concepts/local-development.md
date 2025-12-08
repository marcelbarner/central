# Local Development

## Prerequisites

- .NET 10.0 SDK
- Docker Desktop (for Aspire-managed PostgreSQL)
- Node.js 20+ (for Angular frontend)

## Running with Aspire (Recommended)

Aspire orchestrates all services for local development:

```powershell
dotnet run --project src/Central.AppHost/Central.AppHost.csproj
```

This automatically:
- Starts PostgreSQL in a Docker container
- Configures connection strings via service discovery
- Launches the API server
- Provides Aspire Dashboard for monitoring

**Aspire Dashboard**: Check terminal output for URL (typically http://localhost:15888)

See [DATABASE.md](../../DATABASE.md) for more details.

## Alternative: Manual Startup

If you need to run components separately:

### Database Setup

* **Purpose**: Local development orchestration with service discovery
* **Configuration**: Single-file host (`apphost.cs`)
* **Services**: Manages PostgreSQL, API server, and dependencies
* **Dashboard**: Visual monitoring of running services

Run Aspire host:
```powershell
dotnet run --project apphost.cs
```

Aspire automatically:
- Starts PostgreSQL container
- Configures connection strings
- Launches API server
- Provides monitoring dashboard

## Running the Application

### With Aspire (Recommended)

```powershell
dotnet run --project apphost.cs
```

Access points:
- **Aspire Dashboard**: http://localhost:15888 (check terminal for exact URL)
- **API**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

### Backend Only (Manual)

If running without Aspire, ensure PostgreSQL is available and connection string is configured:

```powershell
# Set connection string
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=central_dev;Username=postgres;Password=postgres"

# Run the application
dotnet run --project src/Central.Server/Central.Server.csproj
```

The API will be available at `https://localhost:5001`.

**Automatic on startup**:
- Database migrations are applied
- Test user is seeded (username: `testuser`, password: `Test123!`)

### Frontend (Angular)

```powershell
# Navigate to client project
cd src/Central.Client

# Install dependencies (first time only)
npm install

# Run development server
npm start
```

The frontend will be available at `http://localhost:4200`.

## Development Workflow

### Using Aspire (Recommended)

1. **Start Aspire**: `dotnet run --project src/Central.AppHost/Central.AppHost.csproj`
   - PostgreSQL and API start automatically
   - Open Aspire Dashboard (URL shown in terminal)
2. **Start Frontend**: `cd src/Central.Client && npm start`
3. **Open Browser**: Navigate to `http://localhost:4200`
4. **Login**: Use test user credentials (testuser / Test123!)

### Manual Workflow

1. **Start PostgreSQL**: Ensure PostgreSQL is running with proper connection string
2. **Start Backend**: `dotnet run --project src/Central.Server/Central.Server.csproj`
3. **Start Frontend**: `cd src/Central.Client && npm start`
4. **Open Browser**: Navigate to `http://localhost:4200`
5. **Login**: Use test user credentials (testuser / Test123!)

## Hot Reload

- **Backend**: .NET hot reload is enabled automatically with `dotnet watch run`
- **Frontend**: Angular dev server provides hot module replacement (HMR)

## Debugging

### Visual Studio Code

Launch configurations are available in `.vscode/launch.json`:
- **Debug Backend**: F5 in VS Code
- **Debug Frontend**: Use browser DevTools

### Visual Studio

Open `Central.slnx` and press F5 to debug.

## Database Management

### View Database

Find connection details in Aspire Dashboard, then connect:
```powershell
# List containers
docker ps

# Using psql (replace container name from above)
docker exec -it <postgres-container-name> psql -U postgres -d centraldb

# Query users
SELECT "Id", "UserName", "Email", "CreatedAt", "LastLoginAt" FROM "Users";
```

### Create Migration

After modifying domain entities:
```powershell
dotnet ef migrations add MigrationName `
  --project src/Central.Infrastructure/Central.Infrastructure.csproj `
  --startup-project src/Central.Server/Central.Server.csproj
```

### Reset Database

```powershell
# Stop Aspire (Ctrl+C in Aspire terminal)

# Remove PostgreSQL volumes
docker volume prune

# Restart Aspire (migrations will recreate schema)
dotnet run --project src/Central.AppHost/Central.AppHost.csproj
```

## Testing

### Run All Tests

```powershell
dotnet test
```

### Run Specific Test Project

```powershell
dotnet test tests/Central.Domain.Tests/Central.Domain.Tests.csproj
dotnet test tests/Central.Server.Tests/Central.Server.Tests.csproj
dotnet test tests/Central.AcceptanceTests/Central.AcceptanceTests.csproj
```

### Run with Coverage

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

## Useful Commands

### Format Code

```powershell
dotnet format
```

### Clean Build

```powershell
dotnet clean
dotnet build
```

### Restore Tools

```powershell
dotnet tool restore
```

### View Swagger/OpenAPI

Navigate to `https://localhost:5001/swagger` when the API is running.

## Troubleshooting

### Port Conflicts

If ports are already in use:

**PostgreSQL (managed by Aspire)**:
- Aspire dynamically assigns ports
- Check Aspire Dashboard for actual port
- To use a specific port, modify `apphost.cs`

**API (5001)**:
- Modify `launchSettings.json` in `src/Central.Server/Properties/`

**Angular (4200)**:
- Use `npm start -- --port 4201`

### Database Connection Issues

1. Check Aspire Dashboard to see if PostgreSQL is running
2. Verify containers: `docker ps`
3. Check logs in Aspire Dashboard or: `docker logs <postgres-container-name>`
4. Restart Aspire: Press `Ctrl+C` and run `dotnet run --project src/Central.AppHost/Central.AppHost.csproj`

### Migration Issues

Reset and reapply:
```powershell
dotnet ef database drop --force `
  --project src/Central.Infrastructure/Central.Infrastructure.csproj `
  --startup-project src/Central.Server/Central.Server.csproj

dotnet ef database update `
  --project src/Central.Infrastructure/Central.Infrastructure.csproj `
  --startup-project src/Central.Server/Central.Server.csproj
```
