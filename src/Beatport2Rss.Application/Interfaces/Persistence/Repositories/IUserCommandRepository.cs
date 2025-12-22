using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface IUserCommandRepository : ICommandRepository<User, UserId>;