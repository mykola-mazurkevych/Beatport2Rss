using Beatport2Rss.ReleaseCollector.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class ReleaseIdValueConverter() :
    ValueConverter<ReleaseId, Guid>(
        releaseId => releaseId.Value,
        value => ReleaseId.Create(value));