using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TagCommandRepository(Beatport2RssDbContext dbContext) :
    CommandRepository<Tag, TagId>(dbContext),
    ITagCommandRepository;