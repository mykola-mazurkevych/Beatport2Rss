using Beatport2Rss.Api.Domain.Tags;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class TagIdValueConverter() :
    ValueConverter<TagId, int>(
        tagId => tagId.Value,
        value => TagId.Create(value));