#pragma warning disable CA1834 // Use StringBuilder.Append(char) for single character strings

using System.Text;

using Beatport2Rss.SourceGenerator.Extensions;
using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator.SourceOutputs;

internal static class ServiceCollectionExtensionsSourceOutput
{
    public static void Add(
        SourceProductionContext context,
        IReadOnlyList<MessageInfo> infos)
    {
        HashSet<string> namespaces =
        [
            "Beatport2Rss.Application.Behaviors",
            "FluentResults",
            "FluentValidation",
            "Mediator",
            "Microsoft.Extensions.DependencyInjection",
        ];
        namespaces.UnionWith(infos.Select(i => i.Namespace));

        var behaviorsBuilder = new StringBuilder();
        var validatorsBuilder = new StringBuilder();

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
                        GenerateValidationBehavior(info, validatorsBuilder);
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
        builder.AppendLine("namespace Beatport2Rss.Application;");
        builder.AppendLine("public static partial class ServiceCollectionExtensions");
        builder.AppendLine("{");

        builder.AppendLine("    private static partial IServiceCollection AddBehaviors(this IServiceCollection services) =>");
        builder.Append("        services");
        builder.Append(behaviorsBuilder);
        builder.AppendLine(";");
        builder.AppendLine();

        builder.AppendLine("    private static partial IServiceCollection AddValidators(this IServiceCollection services) =>");
        builder.Append("        services");
        builder.Append(validatorsBuilder);
        builder.AppendLine(";");
        builder.Append("}");

        context.AddSource("ServiceCollectionExtensions.g.cs", builder.ToString());
    }

    private static void GenerateValidationBehavior(
        string entityName,
        MessageInfo info,
        StringBuilder builder,
        HashSet<string> namespaces)
    {
        var resultSymbol = info.Interfaces.Single(i => i is { Name: "ICommand" or "IQuery" }).TypeArguments.OfType<INamedTypeSymbol>().Single();

        builder.AppendLine();
        builder.Append($"            .AddTransient<IPipelineBehavior<{info.Name}, {resultSymbol.GenerateName(namespaces)}>, {info.Name}{entityName}ValidationBehavior>()");
    }

    private static void GenerateValidationBehavior(
        MessageInfo info,
        StringBuilder builder)
    {
        builder.AppendLine();
        builder.Append($"            .AddSingleton<IValidator<{info.Name}>, {info.Name}Validator>()");
    }
}