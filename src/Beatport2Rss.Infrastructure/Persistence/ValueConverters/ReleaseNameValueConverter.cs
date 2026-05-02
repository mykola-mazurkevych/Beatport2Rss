using Beatport2Rss.Domain.Releases;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class ReleaseNameValueConverter() :
    ValueConverter<ReleaseName, string>(
        releaseName => releaseName.Value,
        value => ReleaseName.Create(value));