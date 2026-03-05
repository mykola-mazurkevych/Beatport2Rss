using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Tags;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class TagCommandRepository(DbSet<Tag> tags) :
    CommandRepository<Tag, TagId>(tags),
    ITagCommandRepository;