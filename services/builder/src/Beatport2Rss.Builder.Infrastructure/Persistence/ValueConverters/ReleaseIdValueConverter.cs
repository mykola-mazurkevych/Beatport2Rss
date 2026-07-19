using Beatport2Rss.Builder.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class ReleaseIdValueConverter() :
    ValueConverter<ReleaseId, Guid>(
        releaseId => releaseId.Value,
        value => ReleaseId.Create(value));