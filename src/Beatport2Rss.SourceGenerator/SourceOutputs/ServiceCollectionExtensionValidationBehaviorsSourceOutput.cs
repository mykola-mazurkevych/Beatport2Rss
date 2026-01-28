using System.Text;

using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator.SourceOutputs;

internal static class ServiceCollectionExtensionValidationBehaviorsSourceOutput
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
                "Beatport2Rss.Application.Behaviors",
                "Beatport2Rss.Domain.Common.ValueObjects",
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
        builder.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        builder.AppendLine();
        builder.AppendLine("namespace Beatport2Rss.Application;");
        builder.AppendLine();

        builder.AppendLine("public static partial class ServiceCollectionExtensions");
        builder.AppendLine("{");
        builder.AppendLine("    private static partial IServiceCollection AddValidationBehaviors(this IServiceCollection services) =>");
        builder.Append("        services");

        foreach (var info in mediatorMessageInfos.OrderBy(i => i.Name))
        {
            builder.AppendLine();
            builder.Append("            .AddSingleton<");

            var messageSymbol = info.Interfaces.Single(i => i is { Name: "ICommand" or "IQuery" });
            var resultSymbol = (INamedTypeSymbol)messageSymbol.TypeArguments.Single();

            if (resultSymbol.IsGenericType)
            {
                var valueSymbol = resultSymbol.TypeArguments.Single();
                builder.Append($"IPipelineBehavior<{info.Name}, {resultSymbol.Name}<{valueSymbol.Name}>>");
            }
            else
            {
                builder.Append($"IPipelineBehavior<{info.Name}, {resultSymbol.Name}>");
            }

            builder.Append(", ");
            builder.Append($"{info.Name}ValidationBehavior");
            builder.Append(">()");
        }

        builder.AppendLine(";");
        builder.Append('}');

        context.AddSource("ServiceCollectionExtensions.ValidationBehaviors.g.cs", builder.ToString());
    }
}