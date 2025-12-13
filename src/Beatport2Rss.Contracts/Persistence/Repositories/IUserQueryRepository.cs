using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Contracts.Persistence.Repositories;

public interface IUserQueryRepository : IQueryRepository<User, UserId>;