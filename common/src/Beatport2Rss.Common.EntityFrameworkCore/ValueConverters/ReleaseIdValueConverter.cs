using Beatport2Rss.Common.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class ReleaseIdValueConverter() :
    ValueConverter<ReleaseId, Guid>(
        releaseId => releaseId.Value,
        value => ReleaseId.Create(value));