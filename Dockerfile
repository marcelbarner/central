# Build stage for Angular frontend
FROM node:22-alpine AS angular-build
WORKDIR /src/client

# Copy package files and install dependencies
COPY src/Central.Client/package*.json ./
RUN npm ci --legacy-peer-deps

# Copy Angular source and build
COPY src/Central.Client/ ./
RUN npm run build -- --configuration production

# Build stage for .NET backend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dotnet-build
WORKDIR /src

# Copy solution and project files
COPY Central.slnx ./
COPY Directory.Build.props ./
COPY Directory.Build.targets ./
COPY Directory.Packages.props ./
COPY global.json ./

# Copy all project files
COPY src/Central.Domain/Central.Domain.csproj ./src/Central.Domain/
COPY src/Central.Infrastructure/Central.Infrastructure.csproj ./src/Central.Infrastructure/
COPY src/Central.Server/Central.Server.csproj ./src/Central.Server/

# Restore dependencies
RUN dotnet restore src/Central.Server/Central.Server.csproj

# Copy source code
COPY src/Central.Domain/ ./src/Central.Domain/
COPY src/Central.Infrastructure/ ./src/Central.Infrastructure/
COPY src/Central.Server/ ./src/Central.Server/

# Copy Angular build output to wwwroot
# Angular 21 outputs to dist/Central.Client/browser/
RUN mkdir -p ./src/Central.Server/wwwroot
COPY --from=angular-build /src/client/dist/Central.Client/browser/ ./src/Central.Server/wwwroot/

# Build and publish
WORKDIR /src/src/Central.Server
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for healthchecks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=dotnet-build /app/publish .

# Create directory for media files
RUN mkdir -p /app/media

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV FileSystem__Media=/app/media

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "Central.Server.dll"]
