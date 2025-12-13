using Beatport2Rss.Contracts.Persistence.Repositories;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserCommandRepository(Beatport2RssDbContext dbContext)
    : CommandRepository<Domain.Users.User, Domain.Users.UserId>(dbContext), IUserCommandRepository;