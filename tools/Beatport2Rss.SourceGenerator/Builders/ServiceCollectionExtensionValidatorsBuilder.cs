#pragma warning disable CA1834 // Use StringBuilder.Append(char) for single character strings

using System.Text;

using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis.Text;

namespace Beatport2Rss.SourceGenerator.Builders;

internal sealed class ServiceCollectionExtensionValidatorsBuilder :
    IBuilder
{
    private readonly StringBuilder _builder = new();

    private readonly HashSet<string> _namespaces =
    [
        "FluentValidation",
        "Microsoft.Extensions.DependencyInjection",
    ];

    public string HintName =>
        "ServiceCollectionExtensions.Validators.g.cs";

    public void Append(MessageInfo info)
    {
        _namespaces.Add(info.Namespace);

        _builder.AppendLine();
        _builder.Append($"            .AddSingleton<IValidator<{info.Name}>, {info.Name}Validator>()");
    }

    public bool CanHandle(string symbolName) =>
        string.Equals(symbolName, "IRequireValidation", StringComparison.OrdinalIgnoreCase);

    public SourceText ToSourceText()
    {
        var sourceTextBuilder = new StringBuilder();
        foreach (var @namespace in _namespaces.OrderBy(n => n))
        {
            sourceTextBuilder.AppendLine($"using {@namespace};");
        }

        sourceTextBuilder.AppendLine();
        sourceTextBuilder.AppendLine("namespace Beatport2Rss.Application;");
        sourceTextBuilder.AppendLine("public static partial class ServiceCollectionExtensions");
        sourceTextBuilder.AppendLine("{");
        sourceTextBuilder.AppendLine("    private static partial IServiceCollection AddValidators(this IServiceCollection services) =>");
        sourceTextBuilder.Append("        services");
        sourceTextBuilder.Append(_builder);
        sourceTextBuilder.AppendLine(";");
        sourceTextBuilder.Append("}");

        return SourceText.From(sourceTextBuilder.ToString(), Encoding.UTF8);
    }
}