using Beatport2Rss.Collector.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class BeatportSlugValueConverter() :
    ValueConverter<BeatportSlug, string>(
        beatportSlug => beatportSlug.Value,
        value => BeatportSlug.Create(value));