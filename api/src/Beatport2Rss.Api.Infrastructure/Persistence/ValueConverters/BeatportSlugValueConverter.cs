using Beatport2Rss.Api.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class BeatportSlugValueConverter() :
    ValueConverter<BeatportSlug, string>(
        beatportSlug => beatportSlug.Value,
        value => BeatportSlug.Create(value));