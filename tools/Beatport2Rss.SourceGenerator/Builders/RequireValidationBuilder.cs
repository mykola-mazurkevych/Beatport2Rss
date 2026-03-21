#pragma warning disable CA1834 // Use StringBuilder.Append(char) for single character strings

using System.Text;

using Beatport2Rss.SourceGenerator.Extensions;
using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Beatport2Rss.SourceGenerator.Builders;

internal sealed class RequireValidationBuilder :
    IBuilder
{
    private readonly StringBuilder _builder = new();

    private readonly HashSet<string> _namespaces =
    [
        "FluentValidation",
        "Mediator",
    ];

    public string HintName =>
        "RequireValidationBehaviors.g.cs";

    public bool CanHandle(string symbolName) =>
        string.Equals(symbolName, "IRequireValidation", StringComparison.OrdinalIgnoreCase);

    public void Append(MessageInfo info)
    {
        _namespaces.Add(info.Namespace);

        var resultSymbol = info.Interfaces.Single(i => i is { Name: "ICommand" or "IQuery" }).TypeArguments.OfType<INamedTypeSymbol>().Single();
        var resultSymbolName = resultSymbol.GenerateName(_namespaces);

        _builder.AppendLine();
        _builder.AppendLine();

        _builder.AppendLine($"internal sealed class {info.Name}RequireValidationBehavior(IValidator<{info.Name}> validator) :");
        _builder.Append($"    RequireValidationBehavior<{info.Name}, {resultSymbolName}");

        if (resultSymbol.IsGenericType)
        {
            var valueSymbolName = resultSymbol.TypeArguments.OfType<INamedTypeSymbol>().Single().GenerateName(_namespaces);
            _builder.Append($", {valueSymbolName}");
        }

        _builder.AppendLine(">(validator),");
        _builder.AppendLine($"    IPipelineBehavior<{info.Name}, {resultSymbolName}>");

        _builder.AppendLine("{");
        _builder.Append("}");
    }

    public SourceText ToSourceText()
    {
        var sourceTextBuilder = new StringBuilder();
        foreach (var @namespace in _namespaces.OrderBy(n => n))
        {
            sourceTextBuilder.AppendLine($"using {@namespace};");
        }

        sourceTextBuilder.AppendLine();
        sourceTextBuilder.Append("namespace Beatport2Rss.Application.Behaviors;");
        sourceTextBuilder.Append(_builder);

        return SourceText.From(sourceTextBuilder.ToString(), Encoding.UTF8);
    }
}