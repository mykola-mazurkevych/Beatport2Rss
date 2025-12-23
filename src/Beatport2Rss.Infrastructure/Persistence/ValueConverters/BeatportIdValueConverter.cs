using Beatport2Rss.Domain.Common.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class BeatportIdValueConverter() : ValueConverter<BeatportId, int>(beatportId => beatportId.Value, value => BeatportId.Create(value));