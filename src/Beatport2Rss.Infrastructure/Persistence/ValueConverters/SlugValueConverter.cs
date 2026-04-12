using Beatport2Rss.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class SlugValueConverter() :
    ValueConverter<Slug, string>(
        slug => slug.Value,
        value => Slug.Create(value));