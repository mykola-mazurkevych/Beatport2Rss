using Beatport2Rss.Api.Domain.Subscriptions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class BeatportIdValueConverter() :
    ValueConverter<BeatportId, int>(
        beatportId => beatportId.Value,
        value => BeatportId.Create(value));