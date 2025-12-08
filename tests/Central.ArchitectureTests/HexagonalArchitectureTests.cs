using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Central.ArchitectureTests;

public class HexagonalArchitectureTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.Meta).Assembly,
            typeof(Infrastructure.Meta).Assembly,
            typeof(Server.Meta).Assembly
        )
        .Build();

    private readonly IObjectProvider<IType> DomainLayer =
        ArchRuleDefinition.Types()
            .That()
            .ResideInNamespaceMatching("Central.Domain")
            .As("Domain Layer");

    private readonly IObjectProvider<IType> InfrastructureLayer =
        ArchRuleDefinition.Types()
            .That()
            .ResideInNamespaceMatching("Central.Infrastructure")
            .As("Infrastructure Layer");

    private readonly IObjectProvider<IType> ApplicationLayer =
        ArchRuleDefinition.Types()
            .That()
            .ResideInNamespaceMatching("Central.Server")
            .As("Application Layer");

    [Fact]
    public void Domain_Should_Not_DependOn_Infrastructure()
    {
        var rule = Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(InfrastructureLayer)
            .Because("Domain is the core and should not depend on infrastructure");

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_DependOn_Application()
    {
        var rule = Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(ApplicationLayer)
            .Because("Domain is the core and should not depend on application layer");

        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_Should_Not_DependOn_Application()
    {
        var rule = ArchRuleDefinition.Types()
            .That()
            .Are(InfrastructureLayer)
            .Should()
            .NotDependOnAny(ApplicationLayer)
            .Because("Infrastructure should only implement ports defined in domain");

        rule.Check(Architecture);
    }

    // [Fact]
    // public void Domain_Should_Not_Have_External_Dependencies()
    // {
    //     var rule = ArchRuleDefinition.Types()
    //         .That()
    //         .Are(DomainLayer)
    //         .Should()
    //         .OnlyDependOn(
    //             "Central.Domain",
    //             "System",
    //             "netstandard",
    //             "mscorlib"
    //         )
    //         .Because("Domain should be free of external dependencies");

    //     rule.Check(Architecture);
    // }
}
