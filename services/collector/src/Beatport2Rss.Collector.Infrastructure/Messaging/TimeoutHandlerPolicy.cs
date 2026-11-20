using System.Collections.Frozen;

using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;

using JasperFx;
using JasperFx.CodeGeneration;

using Wolverine.Configuration;
using Wolverine.Runtime.Handlers;

namespace Beatport2Rss.Collector.Infrastructure.Messaging;

internal sealed class TimeoutHandlerPolicy :
    IHandlerPolicy
{
    private static readonly FrozenDictionary<Type, int> ExecutionTimeouts =
        new Dictionary<Type, int> { { typeof(CollectReleasesCommand), 120 } }
            .ToFrozenDictionary();

    public void Apply(IReadOnlyList<HandlerChain> chains, GenerationRules rules, IServiceContainer container)
    {
        foreach (var chain in chains)
        {
            if (ExecutionTimeouts.TryGetValue(chain.MessageType, out var executionTimeoutInSeconds))
            {
                chain.ExecutionTimeoutInSeconds = executionTimeoutInSeconds;
            }
        }
    }
}