# Persistence

## Entity Framework Core

* **Database**: PostgreSQL
* **Approach**: Code-first with migrations
* **Context**: Single DbContext in Infrastructure layer
* **Configurations**: Fluent API via `IEntityTypeConfiguration<T>`

## Repository Pattern

Repositories in the Infrastructure layer implement port interfaces defined in the Domain layer.

```csharp
// Domain (Port)
public interface IHelloRepository
{
    Task<HelloDomain> GetByIdAsync(int id);
}

// Infrastructure (Adapter)
public class HelloRepository : IHelloRepository
{
    private readonly AppDbContext _context;
    // Implementation using EF Core
}
```
