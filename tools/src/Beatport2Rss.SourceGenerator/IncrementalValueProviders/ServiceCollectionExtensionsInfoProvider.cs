using System.Collections.Immutable;

using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beatport2Rss.SourceGenerator.IncrementalValueProviders;

internal static class ServiceCollectionExtensionsInfoProvider
{
    public static IncrementalValueProvider<ImmutableArray<GenerationTargetInfo>> Provide(
        IncrementalGeneratorInitializationContext context) =>
        context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax
                {
                    AttributeLists.Count: > 0,
                    Identifier.ValueText: "ServiceCollectionExtensions",
                },
                transform: static (context, cancellationToken) =>
                    context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)context.Node, cancellationToken) as INamedTypeSymbol)
            .Where(static symbol => symbol is not null)
            .Collect()
            .Select(static (symbols, _) =>
            {
                var targets = ImmutableArray.CreateBuilder<GenerationTargetInfo>();
                HashSet<string> seenTargets = [];

                foreach (var symbol in symbols)
                {
                    if (symbol is null)
                    {
                        continue;
                    }

                    var features = symbol.GetAttributes()
                        .Select(attribute => GetFeature(attribute.AttributeClass?.ToDisplayString()))
                        .Where(feature => feature is not null)
                        .Select(feature => feature!.Value)
                        .ToImmutableHashSet();

                    var fullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    if (features.Count == 0 || !seenTargets.Add(fullyQualifiedName))
                    {
                        continue;
                    }

                    targets.Add(new GenerationTargetInfo(
                        symbol.Name,
                        symbol.ContainingNamespace.ToDisplayString(),
                        string.Join(
                            " ",
                            symbol.DeclaringSyntaxReferences
                                .Select(reference => reference.GetSyntax())
                                .OfType<ClassDeclarationSyntax>()
                                .First()
                                .Modifiers
                                .Select(modifier => modifier.Text)),
                        symbol.IsStatic,
                        symbol.DeclaringSyntaxReferences
                            .Select(reference => reference.GetSyntax())
                            .OfType<ClassDeclarationSyntax>()
                            .All(declaration => declaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword))),
                        features));
                }

                return targets.ToImmutable();
            });

    private static GeneratedFeature? GetFeature(string? attributeName) => attributeName switch
    {
        "Beatport2Rss.SourceGenerator.GenerateValidatorsAttribute" => GeneratedFeature.Validators,
        "Beatport2Rss.SourceGenerator.GenerateRequireUserBehaviorsAttribute" => GeneratedFeature.RequireUserBehaviors,
        "Beatport2Rss.SourceGenerator.GenerateRequireFeedBehaviorsAttribute" => GeneratedFeature.RequireFeedBehaviors,
        "Beatport2Rss.SourceGenerator.GenerateRequireTagBehaviorsAttribute" => GeneratedFeature.RequireTagBehaviors,
        "Beatport2Rss.SourceGenerator.GenerateRequireSubscriptionBehaviorsAttribute" => GeneratedFeature.RequireSubscriptionBehaviors,
        _ => null,
    };
}