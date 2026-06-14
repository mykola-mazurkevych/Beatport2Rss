using Beatport2Rss.ReleaseCollector.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class BeatportSlugValueConverter() :
    ValueConverter<BeatportSlug, string>(
        beatportSlug => beatportSlug.Value,
        value => BeatportSlug.Create(value));