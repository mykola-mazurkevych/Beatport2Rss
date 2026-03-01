using Beatport2Rss.Domain.Tags;

namespace Beatport2Rss.Application.Interfaces.Persistence.Repositories;

public interface ITagCommandRepository :
    ICommandRepository<Tag, TagId>;