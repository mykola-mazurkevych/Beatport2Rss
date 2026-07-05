using Beatport2Rss.Api.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class SlugValueConverter() :
    ValueConverter<Slug, string>(
        slug => slug.Value,
        value => Slug.Create(value));