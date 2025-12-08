using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Central.ArchitectureTests;

public class StructureTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.Meta).Assembly,
            typeof(Infrastructure.Meta).Assembly,
            typeof(Server.Meta).Assembly
        )
        .Build();

    [Fact]
    public void FastEndpoints_Should_BeIn_FeaturesFolder()
    {
        var rule = Classes()
            .That()
            .HaveNameEndingWith("Endpoint")
            .And()
            .ResideInAssembly("Central.Server")
            .Should()
            .ResideInNamespaceMatching("Central.Server.Features")
            .Because("Endpoints should be organized in Features folder")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void DbContext_Should_BeIn_Infrastructure()
    {
        var rule = Classes()
            .That()
            .HaveNameEndingWith("DbContext")
            .Or()
            .HaveNameEndingWith("Context")
            .Should()
            .ResideInNamespaceMatching("Central.Infrastructure")
            .Because("DbContext should only exist in Infrastructure layer")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }
}
