using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator;

internal static class Diagnostics
{
    internal static readonly DiagnosticDescriptor InvalidTarget = new(
        id: "BP2RSSSG002",
        title: "Invalid generation target",
        messageFormat: "Generation target '{0}' must be declared static",
        category: "Beatport2Rss.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}