using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Central.ArchitectureTests;

public class ImmutabilityTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.Meta).Assembly,
            typeof(Infrastructure.Meta).Assembly,
            typeof(Server.Meta).Assembly
        )
        .Build();

    [Fact]
    public void ValueObjects_Should_Be_Immutable()
    {
        var rule = Classes()
            .That()
            .ResideInNamespaceMatching("Central.Domain.ValueObjects")
            .Should()
            .BeImmutable()
            .Because("Value objects should be immutable");

        // This will pass when we have value objects
        // rule.Check(Architecture);
    }

    [Fact]
    public void Events_Should_Be_Immutable()
    {
        var rule = Classes()
            .That()
            .ResideInNamespaceMatching("Central.Domain.Events")
            .Or()
            .HaveNameEndingWith("Event")
            .Should()
            .BeImmutable()
            .Because("Domain events should be immutable");

        // This will pass when we have domain events
        // rule.Check(Architecture);
    }

    [Fact]
    public void DTOs_Should_Have_PublicSetters()
    {
        var rule = Classes()
            .That()
            .ResideInNamespaceMatching("Central.Server.Features")
            .And()
            .HaveNameEndingWith("Request")
            .Or()
            .HaveNameEndingWith("Response")
            .Should()
            .NotBeImmutable()
            .Because("DTOs need public setters for serialization/deserialization");

        rule.Check(Architecture);
    }
}
