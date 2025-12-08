# System Context

## Business Context

Central provides a web-based application with a RESTful API backend and Angular frontend. The system exposes HTTP endpoints for client interactions.

```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Context.puml

Person(user, "User", "Application user")
System(central, "Central System", "Web application with REST API")
System_Ext(db, "PostgreSQL", "Database")

Rel(user, central, "Uses", "HTTPS")
Rel(central, db, "Reads/Writes", "TCP")
@enduml
```

## Technical Context

| Interface | Description | Technology |
|-----------|-------------|------------|
| Frontend | Angular SPA | TypeScript, Angular |
| API | RESTful HTTP API | FastEndpoints, ASP.NET Core |
| Database | Data persistence | PostgreSQL, EF Core |
| Authentication | Cookie auth, API tokens, Basic auth | ASP.NET Core Identity |
| Development | Local orchestration | Aspire |
