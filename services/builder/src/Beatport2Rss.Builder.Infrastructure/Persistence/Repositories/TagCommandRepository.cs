using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;

internal sealed class TagCommandRepository(DbSet<Tag> tags) :
    CommandRepository<Tag, TagId>(tags),
    ITagCommandRepository;
