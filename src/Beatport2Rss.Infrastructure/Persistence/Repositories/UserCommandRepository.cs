using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserCommandRepository(Beatport2RssDbContext dbContext) :
    CommandRepository<User, UserId>(dbContext),
    IUserCommandRepository;