using System.Text;

using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator.SourceOutputs;

internal static class ServiceCollectionExtensionValidatorsSourceOutput
{
    public static void Add(
        SourceProductionContext context,
        IReadOnlyList<MediatorMessageInfo> mediatorMessageInfos)
    {
        var builder = new StringBuilder();

        var namespaces = mediatorMessageInfos
            .Select(i => i.Namespace)
            .Distinct()
            .OrderBy(@namespace => @namespace)
            .ToList();

        foreach (var @namespace in namespaces)
        {
            builder.AppendLine($"using {@namespace};");
        }

        builder.AppendLine();
        builder.AppendLine("using FluentValidation;");
        builder.AppendLine();
        builder.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        builder.AppendLine();
        builder.AppendLine("namespace Beatport2Rss.Application;");
        builder.AppendLine();

        builder.AppendLine("public static partial class ServiceCollectionExtensions");
        builder.AppendLine("{");
        builder.AppendLine("    private static partial IServiceCollection AddValidators(this IServiceCollection services) =>");
        builder.Append("        services");

        foreach (var info in mediatorMessageInfos.OrderBy(i => i.Name))
        {
            builder.AppendLine();
            builder.Append($"            .AddSingleton<IValidator<{info.Name}>, {info.Name}Validator>()");
        }

        builder.AppendLine(";");
        builder.Append('}');

        context.AddSource("ServiceCollectionExtensions.Validators.g.cs", builder.ToString());
    }
}