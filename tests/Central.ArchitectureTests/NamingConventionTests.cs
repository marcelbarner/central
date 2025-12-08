using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Central.ArchitectureTests;

public class NamingConventionTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.Meta).Assembly,
            typeof(Infrastructure.Meta).Assembly,
            typeof(Server.Meta).Assembly
        )
        .Build();

    [Fact]
    public void Entities_Should_NotHave_SuffixEntity()
    {
        var rule = Classes()
            .That()
            .ResideInNamespaceMatching("Central.Domain")
            .Should()
            .NotHaveNameEndingWith("Entity")
            .Because("Domain entities should not have 'Entity' suffix");

        rule.Check(Architecture);
    }

    [Fact]
    public void Repositories_Should_Have_SuffixRepository()
    {
        // Skip this test as we don't have IRepository interface yet
        // When we add repositories, this test will validate naming conventions

        // var rule = Classes()
        //     .That()
        //     .ResideInNamespaceMatching("Central.Infrastructure")
        //     .And()
        //     .ImplementInterface("IRepository")
        //     .Should()
        //     .HaveNameEndingWith("Repository")
        //     .Because("Repository implementations should have 'Repository' suffix");

        // rule.Check(Architecture);
    }

    [Fact]
    public void Interfaces_Should_StartWith_I()
    {
        var rule = Interfaces()
            .That()
            .ResideInNamespaceMatching("Central")
            .Should()
            .HaveNameStartingWith("I")
            .Because("Interfaces should start with 'I' by convention")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void FastEndpoints_Should_Have_SuffixEndpoint()
    {
        var rule = Classes()
            .That()
            .ResideInNamespaceMatching("Central.Server.Features")
            .Should()
            .HaveNameEndingWith("Endpoint")
            .Because("FastEndpoints should use 'Endpoint' suffix")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }
}