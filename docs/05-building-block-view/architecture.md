# Building Block View

## Level 1: System Overview

Central follows a hexagonal architecture (ports and adapters) pattern with clear separation between domain logic, infrastructure, and application layers.

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Container.puml

Container(client, "Central.Client", "Angular", "Web UI")
Container(server, "Central.Server", "ASP.NET Core", "API layer with FastEndpoints")
Container(domain, "Central.Domain", ".NET", "Core business logic")
Container(infra, "Central.Infrastructure", ".NET", "Database access via EF Core")
ContainerDb(db, "Database", "PostgreSQL", "Persistent data")

Rel(client, server, "Uses", "HTTPS/JSON")
Rel(server, domain, "Uses")
Rel(server, infra, "Uses")
Rel(infra, domain, "Uses")
Rel(infra, db, "Reads/Writes", "EF Core")
@enduml
```

## Level 2: Component Structure

### Central.Server (Application/Adapter Layer)

The API layer exposes HTTP endpoints and orchestrates requests.

* **Endpoints**: FastEndpoints for HTTP request handling
* **DTOs**: Request/Response objects for API contracts
* **Authentication**: JWT token validation
* **Swagger**: API documentation

### Central.Domain (Core/Hexagon)

Pure business logic with no external dependencies.

* **Entities**: Domain models
* **Value Objects**: Immutable domain concepts
* **Domain Services**: Business logic operations
* **Interfaces**: Port definitions for infrastructure

### Central.Infrastructure (Adapter Layer)

Implementation of infrastructure concerns.

* **DbContext**: EF Core database context
* **Repositories**: Data access implementation
* **Entity Configurations**: EF Core mappings
* **Mappers**: Riok.Mapperly static mappers (Domain ↔ Entity ↔ DTO)

### Central.Client (Presentation)

Angular-based frontend application.

* **Components**: UI building blocks
* **Services**: HTTP client for API communication
* **Models**: TypeScript interfaces matching API contracts
* **Internationalization**: ngx-translate for multilingual support

## Dependency Flow

```
Central.Client → Central.Server → Central.Domain ← Central.Infrastructure
                                                   ↓
                                              PostgreSQL
```

**Key principle**: Dependencies point inward. Domain has no dependencies on outer layers.
