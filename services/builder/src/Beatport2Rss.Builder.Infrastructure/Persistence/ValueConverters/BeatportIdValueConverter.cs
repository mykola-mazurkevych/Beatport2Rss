using Beatport2Rss.Builder.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class BeatportIdValueConverter() :
    ValueConverter<BeatportId, int>(
        beatportId => beatportId.Value,
        value => BeatportId.Create(value));