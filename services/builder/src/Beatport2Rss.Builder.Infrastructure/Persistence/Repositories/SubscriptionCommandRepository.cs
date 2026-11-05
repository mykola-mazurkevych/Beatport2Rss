using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionCommandRepository(DbSet<Subscription> subscriptions) :
    CommandRepository<Subscription, SubscriptionId>(subscriptions),
    ISubscriptionCommandRepository;