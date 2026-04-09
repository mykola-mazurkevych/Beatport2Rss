using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.QueryModels.Users;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class UserQueryRepository(
    IQueryable<UserQueryModel> userQueryModels) :
    QueryRepository<UserQueryModel, UserId>(userQueryModels),
    IUserQueryRepository;