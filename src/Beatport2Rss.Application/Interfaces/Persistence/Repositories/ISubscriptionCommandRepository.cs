using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISubscriptionCommandRepository :
    ICommandRepository<Subscription, SubscriptionId>;