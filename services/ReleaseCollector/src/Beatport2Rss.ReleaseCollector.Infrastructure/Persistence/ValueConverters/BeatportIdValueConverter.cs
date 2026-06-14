using Beatport2Rss.ReleaseCollector.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class BeatportIdValueConverter() :
    ValueConverter<BeatportId, int>(
        beatportId => beatportId.Value,
        value => BeatportId.Create(value));