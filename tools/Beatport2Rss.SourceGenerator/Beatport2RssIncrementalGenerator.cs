//// #if DEBUG
//// using System.Diagnostics;
//// #endif

using Beatport2Rss.SourceGenerator.Builders;
using Beatport2Rss.SourceGenerator.IncrementalValueProviders;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator;

[Generator]
public sealed class Beatport2RssIncrementalGenerator :
    IIncrementalGenerator
{
////     public ServiceCollectionExtensionGenerator()
////     {
//// #if DEBUG
////         if (!Debugger.IsAttached)
////         {
////             Debugger.Launch();
////         }
//// #endif
////     }



    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = MediatorMessageInfoProvider.Provide(context, "Mediator.IMessage");
        context.RegisterSourceOutput(
            provider,
            static (ctx, infos) =>
            {
                List<IBuilder> builders =
                [
                    new RequireEntityBuilder("IRequireActiveUser", "User"),
                    new RequireEntityBuilder("IRequireFeed", "Feed"),
                    new RequireEntityBuilder("IRequireSubscription", "Subscription"),
                    new RequireEntityBuilder("IRequireTag", "Tag"),
                    new RequireValidationBuilder(),
                    new ServiceCollectionExtensionRequireEntityBuilder("IRequireActiveUser", "User"),
                    new ServiceCollectionExtensionRequireEntityBuilder("IRequireFeed", "Feed"),
                    new ServiceCollectionExtensionRequireEntityBuilder("IRequireSubscription", "Subscription"),
                    new ServiceCollectionExtensionRequireEntityBuilder("IRequireTag", "Tag"),
                    new ServiceCollectionExtensionValidatorsBuilder(),
                ];

                HashSet<string> interfaceTypeSymbolsToSkip =
                [
                    "ICommand",
                    "IEquatable",
                    "IQuery",
                    "IRequireUser", // TODO: implement support?
                ];

                foreach (var info in infos.OrderBy(i => i.Name))
                {
                    foreach (var interfaceTypeSymbol in info.Interfaces.OrderBy(i => i.Name))
                    {
                        if (interfaceTypeSymbolsToSkip.Contains(interfaceTypeSymbol.Name))
                        {
                            continue;
                        }

                        var supportedBuilders = builders.Where(b => b.CanHandle(interfaceTypeSymbol.Name)).ToList();
                        if (supportedBuilders.Count == 0)
                        {
                            ctx.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnsupportedInterface, location: null, interfaceTypeSymbol.Name));
                            continue;
                        }

                        foreach (var builder in supportedBuilders)
                        {
                            builder.Append(info);
                        }
                    }
                }

                foreach (var builder in builders)
                {
                    ctx.AddSource(builder.HintName, builder.ToSourceText());
                }
            });
    }
}