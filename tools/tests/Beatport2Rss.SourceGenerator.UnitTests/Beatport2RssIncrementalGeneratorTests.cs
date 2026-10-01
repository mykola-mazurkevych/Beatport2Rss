using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Xunit;

namespace Beatport2Rss.SourceGenerator.UnitTests;

public sealed class Beatport2RssIncrementalGeneratorTests
{
    private static readonly ImmutableArray<MetadataReference> References =
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToImmutableArray();

    [Fact]
    public void GeneratesNoFeatureSourcesWithoutOptInAttributes()
    {
        var generatedSources = Generate("""
            namespace Demo.Application
            {
                public static partial class ServiceCollectionExtensions { }
            }
            """);

        Assert.All(generatedSources, source => Assert.Equal("GenerationAttributes.g.cs", source.HintName));
    }

    [Theory]
    [InlineData("GenerateValidators", "IRequireValidation", "Validators", "AddValidators", "AddRequireFeedBehaviors")]
    [InlineData("GenerateRequireUserBehaviors", "IRequireActiveUser", "RequireUser", "AddRequireUserBehaviors", "AddValidators")]
    [InlineData("GenerateRequireFeedBehaviors", "IRequireFeed", "RequireFeed", "AddRequireFeedBehaviors", "AddValidators")]
    [InlineData("GenerateRequireTagBehaviors", "IRequireTag", "RequireTag", "AddRequireTagBehaviors", "AddValidators")]
    [InlineData("GenerateRequireSubscriptionBehaviors", "IRequireSubscription", "RequireSubscription", "AddRequireSubscriptionBehaviors", "AddValidators")]
    public void GeneratesOnlySelectedFeatureInTargetNamespace(
        string attributeName,
        string markerInterface,
        string featureName,
        string expectedMethod,
        string unselectedMethod)
    {
        var generatedSources = Generate(CreateSource(attributeName, markerInterface));
        var generatedText = string.Join(Environment.NewLine, generatedSources.Select(source => source.SourceText.ToString()));

        Assert.Contains($"Add{featureName}", generatedText, StringComparison.Ordinal);
        Assert.Contains(expectedMethod, generatedText, StringComparison.Ordinal);
        Assert.DoesNotContain(unselectedMethod, generatedText, StringComparison.Ordinal);
        Assert.Contains("namespace Demo.Application.Behaviors;", generatedText, StringComparison.Ordinal);
        Assert.Contains("namespace Demo.Application;", generatedText, StringComparison.Ordinal);
    }

    [Fact]
    public void GeneratesMultipleSelectedFeaturesTogether()
    {
        var generatedSources = Generate("""
            using Beatport2Rss.SourceGenerator;

            namespace Mediator
            {
                public interface IMessage { }
                public interface ICommand<TResult> : IMessage { }
            }

            namespace Demo.Application
            {
                public interface IRequireValidation { }
                public interface IRequireFeed { }
                public sealed record Result;
                public sealed record Request : Mediator.ICommand<Result>, IRequireValidation, IRequireFeed;

                [GenerateValidators]
                [GenerateRequireFeedBehaviors]
                public static partial class ServiceCollectionExtensions { }
            }
            """);
        var generatedText = string.Join(Environment.NewLine, generatedSources.Select(source => source.SourceText.ToString()));

        Assert.Contains("AddValidators", generatedText, StringComparison.Ordinal);
        Assert.Contains("AddRequireFeedBehaviors", generatedText, StringComparison.Ordinal);
        Assert.DoesNotContain("AddRequireTagBehaviors", generatedText, StringComparison.Ordinal);
        Assert.Contains("namespace Demo.Application.Behaviors;", generatedText, StringComparison.Ordinal);
    }

    private static ImmutableArray<GeneratedSourceResult> Generate(string source)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp14);
        var compilation = CSharpCompilation.Create(
            "GeneratorTests",
            [CSharpSyntaxTree.ParseText(source, parseOptions)],
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new Beatport2RssIncrementalGenerator().AsSourceGenerator()],
            parseOptions: parseOptions);

        return driver.RunGenerators(compilation).GetRunResult().Results.Single().GeneratedSources;
    }

    private static string CreateSource(string attributeName, string markerInterface) => $$"""
        using Beatport2Rss.SourceGenerator;

        namespace Mediator
        {
            public interface IMessage { }
            public interface ICommand<TResult> : IMessage { }
        }

        namespace Demo.Application
        {
            public interface {{markerInterface}} { }
            public sealed record Result;
            public sealed record Request : Mediator.ICommand<Result>, {{markerInterface}};

            [{{attributeName}}]
            public static partial class ServiceCollectionExtensions { }
        }
        """;
}