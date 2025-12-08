using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Central.ArchitectureTests;

public class DependencyTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.Meta).Assembly,
            typeof(Infrastructure.Meta).Assembly,
            typeof(Server.Meta).Assembly
        )
        .Build();

    [Fact]
    public void EntityFramework_Should_OnlyBeUsedIn_Infrastructure()
    {
        var efTypes = Types()
            .That()
            .ResideInNamespaceMatching("Microsoft.EntityFrameworkCore")
            .As("EntityFrameworkCore");

        var rule = Types()
            .That()
            .ResideInAssembly("Central.Domain")
            .Or()
            .ResideInAssembly("Central.Server")
            .Should()
            .NotDependOnAny(efTypes)
            .Because("EF Core should only be used in Infrastructure layer")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void FastEndpoints_Should_OnlyBeUsedIn_Application()
    {
        var fastEndpointsTypes = Types()
            .That()
            .ResideInNamespaceMatching("FastEndpoints")
            .As("FastEndpoints");

        var rule = Types()
            .That()
            .ResideInAssembly("Central.Domain")
            .Or()
            .ResideInAssembly("Central.Infrastructure")
            .Should()
            .NotDependOnAny(fastEndpointsTypes)
            .Because("FastEndpoints should only be used in Application layer")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_NotReference_AspNetCore()
    {
        var aspNetCoreTypes = Types()
            .That()
            .ResideInNamespaceMatching("Microsoft.AspNetCore")
            .As("AspNetCore");

        var rule = Types()
            .That()
            .ResideInAssembly("Central.Domain")
            .Should()
            .NotDependOnAny(aspNetCoreTypes)
            .Because("Domain should not depend on web framework")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_Should_NotReference_AspNetCore()
    {
        var aspNetCoreTypes = Types()
            .That()
            .ResideInNamespaceMatching("Microsoft.AspNetCore")
            .As("AspNetCore");

        var rule = Types()
            .That()
            .ResideInAssembly("Central.Infrastructure")
            .Should()
            .NotDependOnAny(aspNetCoreTypes)
            .Because("Infrastructure should not depend on web framework")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }
}
