using Beatport2Rss.Common.Domain.Tags;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class TagIdValueConverter() :
    ValueConverter<TagId, Guid>(
        tagId => tagId.Value,
        value => TagId.Create(value));