using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator.Extensions;

internal static class NamedTypeSymbolExtensions
{
    extension(INamedTypeSymbol namedTypeSymbol)
    {
        public string GenerateName(HashSet<string> namespaces)
        {
            namespaces.Add(namedTypeSymbol.ContainingNamespace.ToString());

            var name = namedTypeSymbol.Name;

            if (namedTypeSymbol.IsGenericType)
            {
                name += '<' + string.Join(", ", namedTypeSymbol.TypeArguments.OfType<INamedTypeSymbol>().Select(typeSymbol => typeSymbol.GenerateName(namespaces))) + '>';
            }

            return name;
        }
    }
}