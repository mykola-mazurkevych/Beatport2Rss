using System.Collections.Immutable;

using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beatport2Rss.SourceGenerator.IncrementalValueProviders;

internal static class MediatorMessageInfoProvider
{
    public static IncrementalValueProvider<ImmutableArray<MediatorMessageInfo>> Provide(
        IncrementalGeneratorInitializationContext context,
        string metadataName) =>
        context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is RecordDeclarationSyntax,
                transform: static (context, cancellationToken) => context.SemanticModel.GetDeclaredSymbol((RecordDeclarationSyntax)context.Node, cancellationToken) as INamedTypeSymbol)
            .Where(static symbol => symbol is not null)
            .Combine(context.CompilationProvider.Select((compilation, _) => compilation.GetTypeByMetadataName(metadataName)))
            .Select(static (pair, _) =>
            {
                var recordSymbol = pair.Left!;
                var iMessageSymbol = pair.Right!;

                foreach (var @interface in recordSymbol.AllInterfaces)
                {
                    if (@interface.OriginalDefinition.Equals(iMessageSymbol, SymbolEqualityComparer.Default))
                    {
                        return new MediatorMessageInfo(
                            recordSymbol.Name,
                            recordSymbol.ContainingNamespace.ToDisplayString(),
                            recordSymbol.Interfaces);
                    }
                }

                return null;
            })
            .Where(static messageInfo => messageInfo is not null)
            .Select(static (messageInfo, _) => messageInfo!)
            .Collect();
}