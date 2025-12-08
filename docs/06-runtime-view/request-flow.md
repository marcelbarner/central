# Runtime View

## Typical Request Flow

This diagram shows how a typical API request flows through the hexagonal architecture layers.

```plantuml
@startuml
participant "Angular Client" as Client
participant "FastEndpoint" as Endpoint
participant "Mapper" as Mapper
participant "Domain Service" as Domain
participant "Repository" as Repo
database "PostgreSQL" as DB

Client -> Endpoint: HTTP POST /api/hello
activate Endpoint

Endpoint -> Endpoint: Validate Request DTO
Endpoint -> Mapper: Map DTO → Domain
activate Mapper
Mapper --> Endpoint: Domain Object
deactivate Mapper

Endpoint -> Domain: Execute Business Logic
activate Domain
Domain -> Repo: Query/Save Data
activate Repo
Repo -> DB: EF Core Query
DB --> Repo: Entity
Repo -> Mapper: Map Entity → Domain
activate Mapper
Mapper --> Repo: Domain Object
deactivate Mapper
Repo --> Domain: Domain Object
deactivate Repo
Domain --> Endpoint: Result
deactivate Domain

Endpoint -> Mapper: Map Domain → Response DTO
activate Mapper
Mapper --> Endpoint: Response DTO
deactivate Mapper

Endpoint --> Client: HTTP 200 OK + JSON
deactivate Endpoint
@enduml
```

## Request Processing Steps

1. **Client Request**: Angular client sends HTTP request with JSON payload
2. **Validation**: FastEndpoints validates request DTO using FluentValidation
3. **DTO → Domain Mapping**: Riok.Mapperly mapper converts DTO to domain object
4. **Business Logic**: Domain service executes core business rules
5. **Data Access**: Repository retrieves/persists data via EF Core
6. **Entity ↔ Domain Mapping**: Mapper converts between database entities and domain objects
7. **Domain → DTO Mapping**: Mapper converts domain result to response DTO
8. **Response**: FastEndpoints sends HTTP response with JSON payload

## Local Development Flow

```plantuml
@startuml
actor Developer
participant "Aspire Host" as Aspire
participant "Central.Server" as Server
participant "PostgreSQL" as DB

Developer -> Aspire: dotnet run (apphost.cs)
activate Aspire
Aspire -> DB: Start PostgreSQL container
activate DB
Aspire -> Server: Start API with connection string
activate Server
Server --> Developer: API ready at https://localhost:xxxx
Developer -> Server: Test API endpoints
Server -> DB: Execute queries
DB --> Server: Results
Server --> Developer: Response
deactivate Server
deactivate DB
deactivate Aspire
@enduml
```
