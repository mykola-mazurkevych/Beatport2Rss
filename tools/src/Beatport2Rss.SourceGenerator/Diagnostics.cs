using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator;

internal static class Diagnostics
{
    internal static readonly DiagnosticDescriptor UnsupportedInterface = new(
        id: "BP2RSSSG001",
        title: "Unsupported interface",
        messageFormat: "Interface '{0}' is not supported by the source generator",
        category: "Beatport2Rss.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}