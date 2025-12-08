# Object Mapping

## Riok.Mapperly

All mappings between layers use Riok.Mapperly for compile-time safe, high-performance conversions.

**Mapping Strategy**: `MapperStrategy.Target` (required)

## Mapping Types

* **DTO ↔ Domain**: API request/response objects to domain models
* **Entity ↔ Domain**: Database entities to domain models
* **Domain ↔ DTO**: Domain results back to API responses

## Example

```csharp
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class HelloMapper
{
    public static partial HelloDomain ToDomain(this HelloRequest dto);
    public static partial HelloResponse ToDto(this HelloDomain domain);
}
```
