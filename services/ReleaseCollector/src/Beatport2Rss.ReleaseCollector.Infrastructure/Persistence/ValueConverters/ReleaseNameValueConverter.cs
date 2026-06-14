using Beatport2Rss.ReleaseCollector.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class ReleaseNameValueConverter() :
    ValueConverter<ReleaseName, string>(
        releaseName => releaseName.Value,
        value => ReleaseName.Create(value));