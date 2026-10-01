using System.Collections.Immutable;

namespace Beatport2Rss.SourceGenerator.Models;

internal sealed record GenerationTargetInfo(
    string TypeName,
    string Namespace,
    string Modifiers,
    bool IsStatic,
    bool IsPartial,
    ImmutableHashSet<GeneratedFeature> Features);