//// #if DEBUG
//// using System.Diagnostics;
//// #endif

using Beatport2Rss.SourceGenerator.Builders;
using Beatport2Rss.SourceGenerator.IncrementalValueProviders;
using Beatport2Rss.SourceGenerator.Models;

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
        context.RegisterPostInitializationOutput(static ctx =>
            ctx.AddSource(
                "GenerationAttributes.g.cs",
                """
                using System;

                namespace Beatport2Rss.SourceGenerator;

                [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                internal sealed class GenerateValidatorsAttribute : Attribute { }

                [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                internal sealed class GenerateRequireUserBehaviorsAttribute : Attribute { }

                [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                internal sealed class GenerateRequireFeedBehaviorsAttribute : Attribute { }

                [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                internal sealed class GenerateRequireTagBehaviorsAttribute : Attribute { }

                [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                internal sealed class GenerateRequireSubscriptionBehaviorsAttribute : Attribute { }
                """));

        var provider = MediatorMessageInfoProvider.Provide(context, "Mediator.IMessage");
        var targetsProvider = ServiceCollectionExtensionsInfoProvider.Provide(context);
        context.RegisterSourceOutput(
            provider.Combine(targetsProvider),
            static (ctx, source) =>
            {
                HashSet<string> interfaceTypeSymbolsToSkip =
                [
                    "ICommand",
                    "IEquatable",
                    "IQuery",
                    "IRequireUser", // TODO: implement support?
                ];

                HashSet<string> generatedInterfaceTypeSymbols =
                [
                    "IRequireActiveUser",
                    "IRequireFeed",
                    "IRequireSubscription",
                    "IRequireTag",
                    "IRequireValidation",
                ];

                var (infos, targets) = source;
                foreach (var target in targets)
                {
                    List<IBuilder> builders = [];

                    if (target.Features.Contains(GeneratedFeature.RequireUserBehaviors))
                    {
                        builders.Add(new RequireEntityBuilder("IRequireActiveUser", "User", target.Namespace));
                        builders.Add(new ServiceCollectionExtensionRequireEntityBuilder("IRequireActiveUser", "User", target.Namespace, target.TypeName));
                    }

                    if (target.Features.Contains(GeneratedFeature.RequireFeedBehaviors))
                    {
                        builders.Add(new RequireEntityBuilder("IRequireFeed", "Feed", target.Namespace));
                        builders.Add(new ServiceCollectionExtensionRequireEntityBuilder("IRequireFeed", "Feed", target.Namespace, target.TypeName));
                    }

                    if (target.Features.Contains(GeneratedFeature.RequireSubscriptionBehaviors))
                    {
                        builders.Add(new RequireEntityBuilder("IRequireSubscription", "Subscription", target.Namespace));
                        builders.Add(new ServiceCollectionExtensionRequireEntityBuilder("IRequireSubscription", "Subscription", target.Namespace, target.TypeName));
                    }

                    if (target.Features.Contains(GeneratedFeature.RequireTagBehaviors))
                    {
                        builders.Add(new RequireEntityBuilder("IRequireTag", "Tag", target.Namespace));
                        builders.Add(new ServiceCollectionExtensionRequireEntityBuilder("IRequireTag", "Tag", target.Namespace, target.TypeName));
                    }

                    if (target.Features.Contains(GeneratedFeature.Validators))
                    {
                        builders.Add(new RequireValidationBuilder(target.Namespace));
                        builders.Add(new ServiceCollectionExtensionValidatorsBuilder(target.Namespace, target.TypeName));
                    }

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
                                if (!generatedInterfaceTypeSymbols.Contains(interfaceTypeSymbol.Name))
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnsupportedInterface, location: null, interfaceTypeSymbol.Name));
                                }

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
                }
            });
    }
}