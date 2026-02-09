using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Tokens;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TokenCommandRepository(Beatport2RssDbContext dbContext) :
    CommandRepository<Token, TokenId>(dbContext), ITokenCommandRepository;