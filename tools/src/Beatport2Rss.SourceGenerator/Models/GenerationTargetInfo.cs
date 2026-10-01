using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator.Models;

internal sealed record GenerationTargetInfo(
    string TypeName,
    string Namespace,
    string Modifiers,
    bool IsStatic,
    bool IsPartial,
    bool IsTopLevel,
    bool IsGeneric,
    bool IsFileLocal,
    bool IsGlobalNamespace,
    Location Location,
    ImmutableHashSet<GeneratedFeature> Features);