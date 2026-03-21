using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator.Models;

internal sealed record MessageInfo(
    string Name,
    string Namespace,
    ImmutableArray<INamedTypeSymbol> Interfaces);