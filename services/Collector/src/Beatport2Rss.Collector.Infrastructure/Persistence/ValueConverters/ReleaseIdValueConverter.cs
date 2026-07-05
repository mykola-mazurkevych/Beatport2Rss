using Beatport2Rss.Collector.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class ReleaseIdValueConverter() :
    ValueConverter<ReleaseId, Guid>(
        releaseId => releaseId.Value,
        value => ReleaseId.Create(value));