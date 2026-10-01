using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Beatport2Rss.SourceGenerator.Builders;

internal interface IBuilder
{
    string HintName { get; }

    bool CanHandle(INamedTypeSymbol interfaceSymbol);

    void Append(MessageInfo info);
    SourceText ToSourceText();
}