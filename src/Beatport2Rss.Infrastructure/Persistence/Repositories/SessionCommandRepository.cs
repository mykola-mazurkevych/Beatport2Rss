using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionCommandRepository(Beatport2RssDbContext dbContext) :
    CommandRepository<Session, SessionId>(dbContext),
    ISessionCommandRepository;