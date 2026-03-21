#pragma warning disable CA1834 // Use StringBuilder.Append(char) for single character strings

using System.Text;

using Beatport2Rss.SourceGenerator.Extensions;
using Beatport2Rss.SourceGenerator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Beatport2Rss.SourceGenerator.Builders;

internal sealed class ServiceCollectionExtensionRequireEntityBuilder(
    string supportedSymbolName,
    string entityName) :
    IBuilder
{
    private readonly StringBuilder _builder = new();

    private readonly HashSet<string> _namespaces =
    [
        "Beatport2Rss.Application.Behaviors",
        "FluentResults",
        "FluentValidation",
        "Mediator",
        "Microsoft.Extensions.DependencyInjection",
    ];

    public string HintName =>
        $"ServiceCollectionExtensions.Require{entityName}Behaviors.g.cs";

    public bool CanHandle(string symbolName) =>
        string.Equals(symbolName, supportedSymbolName, StringComparison.OrdinalIgnoreCase);

    public void Append(MessageInfo info)
    {
        _namespaces.Add(info.Namespace);

        var resultSymbol = info.Interfaces.Single(i => i is { Name: "ICommand" or "IQuery" }).TypeArguments.OfType<INamedTypeSymbol>().Single();
        var resultSymbolName = resultSymbol.GenerateName(_namespaces);

        _builder.AppendLine();
        _builder.Append($"            .AddTransient<IPipelineBehavior<{info.Name}, {resultSymbolName}>, {info.Name}Require{entityName}Behavior>()");
    }

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
        sourceTextBuilder.AppendLine($"    private static partial IServiceCollection AddRequire{entityName}Behaviors(this IServiceCollection services) =>");
        sourceTextBuilder.Append("        services");
        sourceTextBuilder.Append(_builder);
        sourceTextBuilder.AppendLine(";");
        sourceTextBuilder.Append("}");

        return SourceText.From(sourceTextBuilder.ToString(), Encoding.UTF8);
    }
}