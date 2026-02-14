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
        var requireValidationMessages = MediatorMessageInfoProvider.Provide(context, "Beatport2Rss.Application.Interfaces.Messages.IRequireValidation");
        context.RegisterSourceOutput(requireValidationMessages, static (ctx, infos) => ValidationBehaviorsSourceOutput.Add(ctx, infos));
        context.RegisterSourceOutput(requireValidationMessages, static (ctx, infos) => ServiceCollectionExtensionValidationBehaviorsSourceOutput.Add(ctx, infos));
        context.RegisterSourceOutput(requireValidationMessages, static (ctx, infos) => ServiceCollectionExtensionValidatorsSourceOutput.Add(ctx, infos));

        var requireActiveUserMessages = MediatorMessageInfoProvider.Provide(context, "Beatport2Rss.Application.Interfaces.Messages.IRequireActiveUser");
        context.RegisterSourceOutput(requireActiveUserMessages, static (ctx, infos) => UserValidationBehaviorsSourceOutput.Add(ctx, infos));
        context.RegisterSourceOutput(requireActiveUserMessages, static (ctx, infos) => ServiceCollectionExtensionUserValidationBehaviorsSourceOutput.Add(ctx, infos));

        var requireFeedMessages = MediatorMessageInfoProvider.Provide(context, "Beatport2Rss.Application.Interfaces.Messages.IRequireFeed");
        context.RegisterSourceOutput(requireFeedMessages, static (ctx, infos) => FeedValidationBehaviorsSourceOutput.Add(ctx, infos));
        context.RegisterSourceOutput(requireFeedMessages, static (ctx, infos) => ServiceCollectionExtensionFeedValidationBehaviorsSourceOutput.Add(ctx, infos));

        var requireTagMessages = MediatorMessageInfoProvider.Provide(context, "Beatport2Rss.Application.Interfaces.Messages.IRequireTag");
        context.RegisterSourceOutput(requireTagMessages, static (ctx, infos) => TagValidationBehaviorsSourceOutput.Add(ctx, infos));
        context.RegisterSourceOutput(requireTagMessages, static (ctx, infos) => ServiceCollectionExtensionTagValidationBehaviorsSourceOutput.Add(ctx, infos));
    }
}