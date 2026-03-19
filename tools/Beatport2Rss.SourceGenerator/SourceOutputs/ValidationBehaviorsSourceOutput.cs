using System.Text;

using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator.SourceOutputs;

internal static class ValidationBehaviorsSourceOutput
{
    public static void Add(
        SourceProductionContext context,
        IReadOnlyList<MediatorMessageInfo> mediatorMessageInfos)
    {
        var builder = new StringBuilder();

        var namespaces = mediatorMessageInfos
            .Select(i => i.Namespace)
            .Union(
            [
                "Beatport2Rss.Application.Dtos.Sessions",
                "Beatport2Rss.Application.Dtos.Subscriptions",
                "Beatport2Rss.Domain.Common.ValueObjects",
                "Beatport2Rss.Application.Pagination",
                "Beatport2Rss.Application.Dtos.Feeds",
                "Beatport2Rss.Application.Dtos.Tags",
            ])
            .Distinct()
            .OrderBy(@namespace => @namespace)
            .ToList();

        foreach (var @namespace in namespaces)
        {
            builder.AppendLine($"using {@namespace};");
        }

        builder.AppendLine();
        builder.AppendLine("using FluentResults;");
        builder.AppendLine();
        builder.AppendLine("using FluentValidation;");
        builder.AppendLine();
        builder.AppendLine("using Mediator;");
        builder.AppendLine();
        builder.Append("namespace Beatport2Rss.Application.Behaviors;");

        foreach (var info in mediatorMessageInfos.OrderBy(i => i.Name))
        {
            builder.AppendLine();
            builder.AppendLine();

            builder.AppendLine($"internal sealed class {info.Name}ValidationBehavior(IValidator<{info.Name}> validator) :");

            var messageSymbol = info.Interfaces.Single(i => i is { Name: "ICommand" or "IQuery" });
            var resultSymbol = (INamedTypeSymbol)messageSymbol.TypeArguments.Single();

            if (resultSymbol.IsGenericType)
            {
                var valueSymbol = (INamedTypeSymbol)resultSymbol.TypeArguments.Single();
                builder.AppendLine($"    ValidationBehavior<{info.Name}, {GetSymbolName(resultSymbol)}, {GetSymbolName(valueSymbol)}>(validator),");
                builder.AppendLine($"    IPipelineBehavior<{info.Name}, {GetSymbolName(resultSymbol)}>");
            }
            else
            {
                builder.AppendLine($"    ValidationBehavior<{info.Name}, {GetSymbolName(resultSymbol)}>(validator),");
                builder.AppendLine($"    IPipelineBehavior<{info.Name}, {GetSymbolName(resultSymbol)}>");
            }

            builder.AppendLine("{");
            builder.Append('}');
        }

        context.AddSource("ValidationBehaviors.g.cs", builder.ToString());
    }

    private static string GetSymbolName(INamedTypeSymbol symbol)
    {
        var name = symbol.Name;

        if (symbol.IsGenericType)
        {
            name += '<' + string.Join(", ", symbol.TypeArguments.Select(t => GetSymbolName((INamedTypeSymbol)t))) + '>';
        }

        return name;
    }
}