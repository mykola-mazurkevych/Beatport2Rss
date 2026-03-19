#pragma warning disable CA1834 // Use StringBuilder.Append(char) for single character strings

using System.Text;

using Beatport2Rss.SourceGenerator.Extensions;
using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator.SourceOutputs;

internal static class BehaviorsSourceOutput
{
    public static void Add(
        SourceProductionContext context,
        IReadOnlyList<MessageInfo> infos)
    {
        HashSet<string> namespaces =
        [
            "Beatport2Rss.Application.Interfaces.Persistence.Repositories",
            "FluentValidation",
            "Mediator",
        ];

        var behaviorsBuilder = new StringBuilder();

        foreach (var info in infos.OrderBy(i => i.Name))
        {
            namespaces.Add(info.Namespace);

            foreach (var interfaceTypeSymbol in info.Interfaces.OrderBy(i => i.Name))
            {
                switch (interfaceTypeSymbol.Name)
                {
                    case "ICommand":
                    case "IEquatable":
                    case "IQuery":
                        break;
                    case "IRequireActiveUser":
                        GenerateValidationBehavior("User", info, behaviorsBuilder, namespaces);
                        break;
                    case "IRequireFeed":
                        GenerateValidationBehavior("Feed", info, behaviorsBuilder, namespaces);
                        break;
                    case "IRequireSubscription":
                        GenerateValidationBehavior("Subscription", info, behaviorsBuilder, namespaces);
                        break;
                    case "IRequireTag":
                        GenerateValidationBehavior("Tag", info, behaviorsBuilder, namespaces);
                        break;
                    case "IRequireUser":
                        break; // TODO: implement
                    case "IRequireValidation":
                        GenerateValidationBehavior(info, behaviorsBuilder, namespaces);
                        break;
                    default:
                        throw new NotSupportedException($"Interface '{interfaceTypeSymbol.Name}' is not supported.");
                }
            }
        }

        var builder = new StringBuilder();
        foreach (var @namespace in namespaces.OrderBy(n => n))
        {
            builder.AppendLine($"using {@namespace};");
        }

        builder.AppendLine();
        builder.Append("namespace Beatport2Rss.Application.Behaviors;");
        builder.AppendLine(behaviorsBuilder.ToString());

        context.AddSource("Behaviors.g.cs", builder.ToString());
    }

    private static void GenerateValidationBehavior(
        string entityName,
        MessageInfo info,
        StringBuilder builder,
        HashSet<string> namespaces)
    {
        var resultSymbol = info.Interfaces.Single(i => i is { Name: "ICommand" or "IQuery" }).TypeArguments.OfType<INamedTypeSymbol>().Single();

        builder.AppendLine();
        builder.AppendLine();

        builder.AppendLine($"internal sealed class {info.Name}{entityName}ValidationBehavior(I{entityName}QueryRepository repository) :");
        builder.Append($"    {entityName}ValidationBehavior<{info.Name}, {resultSymbol.GenerateName(namespaces)}");

        if (resultSymbol.IsGenericType)
        {
            var valueSymbol = resultSymbol.TypeArguments.OfType<INamedTypeSymbol>().Single();
            builder.Append($", {valueSymbol.GenerateName(namespaces)}");
        }

        builder.AppendLine(">(repository),");
        builder.AppendLine($"    IPipelineBehavior<{info.Name}, {resultSymbol.GenerateName(namespaces)}>");

        builder.AppendLine("{");
        builder.Append("}");
    }

    private static void GenerateValidationBehavior(
        MessageInfo info,
        StringBuilder builder,
        HashSet<string> namespaces)
    {
        var resultSymbol = info.Interfaces.Single(i => i is { Name: "ICommand" or "IQuery" }).TypeArguments.OfType<INamedTypeSymbol>().Single();

        builder.AppendLine();
        builder.AppendLine();

        builder.AppendLine($"internal sealed class {info.Name}ValidationBehavior(IValidator<{info.Name}> validator) :");
        builder.Append($"    ValidationBehavior<{info.Name}, {resultSymbol.GenerateName(namespaces)}");

        if (resultSymbol.IsGenericType)
        {
            var valueSymbol = (INamedTypeSymbol)resultSymbol.TypeArguments.Single();
            builder.Append($", {valueSymbol.GenerateName(namespaces)}");
        }

        builder.AppendLine(">(validator),");
        builder.AppendLine($"    IPipelineBehavior<{info.Name}, {resultSymbol.GenerateName(namespaces)}>");

        builder.AppendLine("{");
        builder.Append("}");
    }
}