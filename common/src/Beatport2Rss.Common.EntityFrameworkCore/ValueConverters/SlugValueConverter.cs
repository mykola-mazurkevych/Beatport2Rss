using Beatport2Rss.Common.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class SlugValueConverter() :
    ValueConverter<Slug, string>(
        slug => slug.Value,
        value => Slug.Create(value));