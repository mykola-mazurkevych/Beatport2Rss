using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Xunit;

namespace Beatport2Rss.SourceGenerator.UnitTests;

public sealed class Beatport2RssIncrementalGeneratorTests
{
    private static readonly ImmutableArray<MetadataReference> References =
    [
        .. ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
        .Split(Path.PathSeparator)
        .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
    ];

    [Fact]
    public void GeneratesNoFeatureSourcesWithoutOptInAttributes()
    {
        var generatedSources = Generate("""
            namespace Demo.Application
            {
                public static partial class ServiceCollectionExtensions { }
            }
            """);

        Assert.Empty(generatedSources.Errors);
        Assert.All(generatedSources.GeneratedSources, source => Assert.Equal("GenerationAttributes.g.cs", source.HintName));
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
        var generatedText = string.Join(Environment.NewLine, generatedSources.GeneratedSources.Select(source => source.SourceText.ToString()));

        Assert.Empty(generatedSources.Errors);
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
            using Demo.Application.Interfaces.Messages;
            using Microsoft.Extensions.DependencyInjection;

            namespace Demo.Application
            {
                public sealed record Result;
                public sealed record Request : Mediator.ICommand<Result>, IRequireValidation, IRequireFeed, IAudited;
                public interface IAudited { }
                internal sealed class RequestValidator : FluentValidation.IValidator<Request> { }

                [GenerateValidators]
                [GenerateRequireFeedBehaviors]
                public static partial class ServiceCollectionExtensions
                {
                    private static partial IServiceCollection AddValidators(this IServiceCollection services);
                    private static partial IServiceCollection AddRequireFeedBehaviors(this IServiceCollection services);
                }
            }
            """);
        var generatedText = string.Join(Environment.NewLine, generatedSources.GeneratedSources.Select(source => source.SourceText.ToString()));

        Assert.Empty(generatedSources.Errors);
        Assert.Contains("AddValidators", generatedText, StringComparison.Ordinal);
        Assert.Contains("AddRequireFeedBehaviors", generatedText, StringComparison.Ordinal);
        Assert.DoesNotContain("AddRequireTagBehaviors", generatedText, StringComparison.Ordinal);
        Assert.Contains("namespace Demo.Application.Behaviors;", generatedText, StringComparison.Ordinal);
    }

    [Fact]
    public void PreservesInternalStaticTargetModifiers()
    {
        var generatedSources = Generate(CreateSource("GenerateValidators", "IRequireValidation", "internal"));
        var generatedText = string.Join(Environment.NewLine, generatedSources.GeneratedSources.Select(source => source.SourceText.ToString()));

        Assert.Empty(generatedSources.Errors);
        Assert.Contains("internal static partial class ServiceCollectionExtensions", generatedText, StringComparison.Ordinal);
    }

    [Fact]
    public void ReportsDiagnosticAndSkipsGenerationForNonStaticTarget()
    {
        AssertInvalidTarget("""
            using Beatport2Rss.SourceGenerator;

            namespace Demo.Application
            {
                [GenerateValidators]
                public partial class ServiceCollectionExtensions { }
            }
            """);
    }

    [Fact]
    public void ReportsDiagnosticAndSkipsGenerationForNonPartialTarget()
    {
        AssertInvalidTarget("""
            using Beatport2Rss.SourceGenerator;

            namespace Demo.Application
            {
                [GenerateValidators]
                public static class ServiceCollectionExtensions { }
            }
            """);
    }

    [Fact]
    public void ReportsDiagnosticAndSkipsGenerationForGenericTarget()
    {
        AssertInvalidTarget("""
            using Beatport2Rss.SourceGenerator;

            namespace Demo.Application
            {
                [GenerateValidators]
                public static partial class ServiceCollectionExtensions<T> { }
            }
            """);
    }

    [Fact]
    public void ReportsDiagnosticAndSkipsGenerationForNestedTarget()
    {
        AssertInvalidTarget("""
            using Beatport2Rss.SourceGenerator;

            namespace Demo.Application
            {
                public class Outer
                {
                    [GenerateValidators]
                    public static partial class ServiceCollectionExtensions { }
                }
            }
            """);
    }

    [Fact]
    public void ReportsDiagnosticAndSkipsGenerationForFileLocalTarget()
    {
        AssertInvalidTarget("""
            using Beatport2Rss.SourceGenerator;

            namespace Demo.Application
            {
                [GenerateValidators]
                file static partial class ServiceCollectionExtensions { }
            }
            """);
    }

    [Fact]
    public void ReportsDiagnosticAndSkipsGenerationForGlobalNamespaceTarget()
    {
        AssertInvalidTarget("""
            using Beatport2Rss.SourceGenerator;

            [GenerateValidators]
            public static partial class ServiceCollectionExtensions { }
            """);
    }

    [Fact]
    public void IgnoresSameNamedInterfaceFromDifferentNamespace()
    {
        var generatedSources = Generate("""
            using Beatport2Rss.SourceGenerator;
            using Microsoft.Extensions.DependencyInjection;

            namespace Demo.Application
            {
                public interface IRequireFeed { }
                public sealed record Result;
                public sealed record Request : Mediator.ICommand<Result>, IRequireFeed, IAudited;
                public interface IAudited { }

                [GenerateRequireFeedBehaviors]
                public static partial class ServiceCollectionExtensions
                {
                    private static partial IServiceCollection AddRequireFeedBehaviors(this IServiceCollection services);
                }
            }
            """);
        var generatedText = string.Join(Environment.NewLine, generatedSources.GeneratedSources.Select(source => source.SourceText.ToString()));

        Assert.Empty(generatedSources.Errors);
        Assert.DoesNotContain("RequestRequireFeedBehavior", generatedText, StringComparison.Ordinal);
    }

    private static GeneratorTestRun Generate(string source)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp14);
        var compilation = CSharpCompilation.Create(
            "GeneratorTests",
            [CSharpSyntaxTree.ParseText(AddCompilationStubs(source), parseOptions)],
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new Beatport2RssIncrementalGenerator().AsSourceGenerator()],
            parseOptions: parseOptions);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

        var compilationErrors = outputCompilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        var errors = generatorDiagnostics
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .Concat(compilationErrors)
            .ToImmutableArray();

        return new GeneratorTestRun(driver.GetRunResult().Results.Single().GeneratedSources, errors);
    }

    private static void AssertInvalidTarget(string source)
    {
        var result = Generate(source);

        var diagnostic = Assert.Single(result.Errors);
        Assert.Equal("BP2RSSSG002", diagnostic.Id);
        Assert.True(diagnostic.Location.IsInSource);
        Assert.All(result.GeneratedSources, generatedSource => Assert.Equal("GenerationAttributes.g.cs", generatedSource.HintName));
    }

    private static string CreateSource(string attributeName, string markerInterface, string accessibility = "public")
    {
        var methodName = attributeName switch
        {
            "GenerateValidators" => "AddValidators",
            "GenerateRequireUserBehaviors" => "AddRequireUserBehaviors",
            "GenerateRequireFeedBehaviors" => "AddRequireFeedBehaviors",
            "GenerateRequireTagBehaviors" => "AddRequireTagBehaviors",
            "GenerateRequireSubscriptionBehaviors" => "AddRequireSubscriptionBehaviors",
            _ => throw new ArgumentOutOfRangeException(nameof(attributeName)),
        };

        return $$"""
        using Beatport2Rss.SourceGenerator;
        using Demo.Application.Interfaces.Messages;
        using Microsoft.Extensions.DependencyInjection;

        namespace Demo.Application
        {
            public sealed record Result;
            public sealed record Request : Mediator.ICommand<Result>, {{markerInterface}}, IAudited;
            public interface IAudited { }
            internal sealed class RequestValidator : FluentValidation.IValidator<Request> { }

            [{{attributeName}}]
            {{accessibility}} static partial class ServiceCollectionExtensions
            {
                private static partial IServiceCollection {{methodName}}(this IServiceCollection services);
            }
        }
        """;
    }

    private static string AddCompilationStubs(string source) => $$"""
        {{source}}

        namespace FluentResults
        {
            public class Result { }
        }

        namespace FluentValidation
        {
            public interface IValidator<TMessage> { }
        }

        namespace Demo.Application.Interfaces.Messages
        {
            public interface IRequireActiveUser { }
            public interface IRequireFeed { }
            public interface IRequireSubscription { }
            public interface IRequireTag { }
            public interface IRequireValidation { }
        }

        namespace Mediator
        {
            public interface IMessage { }
            public interface ICommand<TResult> : IMessage { }
            public interface IPipelineBehavior<TMessage, TResult> { }
        }

        namespace Microsoft.Extensions.DependencyInjection
        {
            public interface IServiceCollection { }

            public static class ServiceCollectionServiceExtensions
            {
                public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services)
                    where TImplementation : TService => services;

                public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services)
                    where TImplementation : TService => services;
            }
        }

        namespace Demo.Application.Interfaces.Persistence.Repositories
        {
            public interface IUserQueryRepository { }
            public interface IFeedQueryRepository { }
            public interface ISubscriptionQueryRepository { }
            public interface ITagQueryRepository { }
        }

        namespace Demo.Application.Behaviors
        {
            using Demo.Application.Interfaces.Persistence.Repositories;
            using FluentValidation;

            internal abstract class RequireUserBehavior<TMessage, TResult>
            {
                protected RequireUserBehavior(IUserQueryRepository repository) { }
            }

            internal abstract class RequireFeedBehavior<TMessage, TResult>
            {
                protected RequireFeedBehavior(IFeedQueryRepository repository) { }
            }

            internal abstract class RequireSubscriptionBehavior<TMessage, TResult>
            {
                protected RequireSubscriptionBehavior(ISubscriptionQueryRepository repository) { }
            }

            internal abstract class RequireTagBehavior<TMessage, TResult>
            {
                protected RequireTagBehavior(ITagQueryRepository repository) { }
            }

            internal abstract class RequireValidationBehavior<TMessage, TResult>
            {
                protected RequireValidationBehavior(IValidator<TMessage> validator) { }
            }
        }

        """;

    private sealed record GeneratorTestRun(
        ImmutableArray<GeneratedSourceResult> GeneratedSources,
        ImmutableArray<Diagnostic> Errors);
}