using Beatport2Rss.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class ReleaseIdValueConverter() : 
    ValueConverter<ReleaseId, int>(
        releaseId => releaseId.Value,
        value => ReleaseId.Create(value));