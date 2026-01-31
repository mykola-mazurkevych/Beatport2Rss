using Beatport2Rss.SourceGenerator.IncrementalValueProviders;
using Beatport2Rss.SourceGenerator.SourceOutputs;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator;

[Generator]
public sealed class ServiceCollectionExtensionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var messages = MediatorMessageInfoProvider.Provide(context, "Mediator.IMessage");

        context.RegisterSourceOutput(messages, static (ctx, mi) => ValidationBehaviorsSourceOutput.Add(ctx, mi));
        context.RegisterSourceOutput(messages, static (ctx, mi) => ServiceCollectionExtensionValidationBehaviorsSourceOutput.Add(ctx, mi));

        context.RegisterSourceOutput(messages, static (ctx, mi) => ServiceCollectionExtensionValidatorsSourceOutput.Add(ctx, mi));

        var activeUserMessages = MediatorMessageInfoProvider.Provide(context, "Beatport2Rss.Application.Interfaces.Messages.IRequireActiveUser");

        context.RegisterSourceOutput(activeUserMessages, static (ctx, mi) => UserValidationBehaviorsSourceOutput.Add(ctx, mi));
        context.RegisterSourceOutput(activeUserMessages, static (ctx, mi) => ServiceCollectionExtensionUserValidationBehaviorsSourceOutput.Add(ctx, mi));
    }
}