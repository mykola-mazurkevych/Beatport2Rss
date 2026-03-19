//// #if DEBUG
//// using System.Diagnostics;
//// #endif

using Beatport2Rss.SourceGenerator.IncrementalValueProviders;
using Beatport2Rss.SourceGenerator.SourceOutputs;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator;

[Generator]
public sealed class ServiceCollectionExtensionGenerator : IIncrementalGenerator
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
        context.RegisterSourceOutput(provider, static (ctx, infos) => BehaviorsSourceOutput.Add(ctx, infos));
        context.RegisterSourceOutput(provider, static (ctx, infos) => ServiceCollectionExtensionsSourceOutput.Add(ctx, infos));
    }
}