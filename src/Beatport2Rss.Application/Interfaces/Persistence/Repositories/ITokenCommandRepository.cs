using Beatport2Rss.Domain.Tokens;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ITokenCommandRepository :
    ICommandRepository<Token, TokenId>;