# Docker Deployment Guide

This guide explains how to run the Central Document Management System using Docker.

## Prerequisites

- Docker Engine 24.0 or later
- Docker Compose V2 or later
- At least 2GB of free disk space

## Quick Start

1. **Clone the repository** (if not already done)

2. **Build and start the services**

```bash
docker-compose up -d
```

This will:
- Build the Angular frontend
- Build the .NET backend
- Start PostgreSQL database
- Start the application
- Run database migrations automatically

3. **Access the application**

Open your browser and navigate to: http://localhost:8080

The application will initialize the database on first startup (may take 30-60 seconds).

## Configuration

### Environment Variables

Copy `.env.example` to `.env` and customize as needed:

```bash
cp .env.example .env
```

**Important**: Change the default password in production!

### Port Configuration

Default ports:
- Application: `8080`
- PostgreSQL: `5432`

To change ports, edit the `ports` section in `docker-compose.yml`:

```yaml
services:
  central-app:
    ports:
      - "8080:8080"  # Change first port: "YOUR_PORT:8080"
```

### Data Persistence

Data is stored in Docker volumes:
- `postgres-data`: Database files
- `media-data`: Uploaded documents

To back up volumes:

```bash
# Backup database
docker exec central-postgres pg_dump -U central central > backup.sql

# Restore database
docker exec -i central-postgres psql -U central central < backup.sql
```

## Docker Commands

### View logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f central-app
docker-compose logs -f postgres
```

### Stop services

```bash
docker-compose down
```

### Stop and remove volumes (⚠️ deletes all data)

```bash
docker-compose down -v
```

### Rebuild after code changes

```bash
docker-compose up -d --build
```

### Check service health

```bash
docker-compose ps
```

## Database Management

### Access PostgreSQL shell

```bash
docker exec -it central-postgres psql -U central -d central
```

### Run migrations manually

Migrations run automatically on startup. To run manually:

```bash
docker exec central-app dotnet ef database update
```

## Troubleshooting

### Application won't start

1. Check logs:
   ```bash
   docker-compose logs central-app
   ```

2. Ensure PostgreSQL is healthy:
   ```bash
   docker-compose ps postgres
   ```

3. Try rebuilding:
   ```bash
   docker-compose down
   docker-compose up -d --build
   ```

### Database connection errors

1. Verify connection string in `docker-compose.yml` matches PostgreSQL credentials
2. Ensure both services are on the same network
3. Check PostgreSQL is ready: `docker-compose logs postgres`

### Port already in use

Change the host port in `docker-compose.yml`:

```yaml
ports:
  - "8081:8080"  # Changed from 8080:8080
```

### Reset everything

```bash
docker-compose down -v
docker-compose up -d --build
```

## Production Deployment

For production deployments:

1. **Change default passwords** in `docker-compose.yml`
2. **Use environment variables** instead of hardcoded values
3. **Enable HTTPS** using a reverse proxy (nginx, Traefik, etc.)
4. **Set up backups** for PostgreSQL volume
5. **Configure logging** with a log aggregation service
6. **Monitor health** endpoints: `/health` and `/alive`
7. **Set resource limits**:

```yaml
services:
  central-app:
    deploy:
      resources:
        limits:
          cpus: '2'
          memory: 1G
        reservations:
          cpus: '1'
          memory: 512M
```

## Development Mode

To run in development mode with live reload:

1. Use the development configuration:
   ```yaml
   environment:
     - ASPNETCORE_ENVIRONMENT=Development
   ```

2. Mount source code:
   ```yaml
   volumes:
     - ./src:/src
   ```

3. Use `docker-compose.dev.yml` (if created) for development-specific overrides

## Architecture

The Docker setup uses a multi-stage build:

1. **angular-build**: Builds Angular frontend in production mode
2. **dotnet-build**: Builds .NET backend and includes Angular output
3. **runtime**: Minimal runtime image with ASP.NET Core

This approach:
- Minimizes final image size
- Includes only necessary runtime dependencies
- Combines frontend and backend in a single container

## Support

For issues or questions:
- Check application logs: `docker-compose logs`
- Review health endpoint: http://localhost:8080/health
- See main README.md for application documentation
