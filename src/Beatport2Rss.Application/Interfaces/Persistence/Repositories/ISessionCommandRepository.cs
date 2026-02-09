using Beatport2Rss.Domain.Sessions;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ISessionCommandRepository :
    ICommandRepository<Session, SessionId>;