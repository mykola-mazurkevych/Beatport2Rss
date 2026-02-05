using Beatport2Rss.SourceGenerator.IncrementalValueProviders;
using Beatport2Rss.SourceGenerator.SourceOutputs;

using Microsoft.CodeAnalysis;

namespace Beatport2Rss.SourceGenerator;

[Generator]
public sealed class ServiceCollectionExtensionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var requireValidationMessages = MediatorMessageInfoProvider.Provide(context, "Beatport2Rss.Application.Interfaces.Messages.IRequireValidation");

        context.RegisterSourceOutput(requireValidationMessages, static (ctx, mi) => ValidationBehaviorsSourceOutput.Add(ctx, mi));
        context.RegisterSourceOutput(requireValidationMessages, static (ctx, mi) => ServiceCollectionExtensionValidationBehaviorsSourceOutput.Add(ctx, mi));

        context.RegisterSourceOutput(requireValidationMessages, static (ctx, mi) => ServiceCollectionExtensionValidatorsSourceOutput.Add(ctx, mi));

        var requireActiveUserMessages = MediatorMessageInfoProvider.Provide(context, "Beatport2Rss.Application.Interfaces.Messages.IRequireActiveUser");

        context.RegisterSourceOutput(requireActiveUserMessages, static (ctx, mi) => UserValidationBehaviorsSourceOutput.Add(ctx, mi));
        context.RegisterSourceOutput(requireActiveUserMessages, static (ctx, mi) => ServiceCollectionExtensionUserValidationBehaviorsSourceOutput.Add(ctx, mi));
    }
}