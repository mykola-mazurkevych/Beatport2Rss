using Beatport2Rss.Domain.Tags;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class TagIdValueConverter() :
    ValueConverter<TagId, int>(
        tagId => tagId.Value,
        value => TagId.Create(value));