using Beatport2Rss.ReleaseCollector.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class MixNameValueConverter() :
    ValueConverter<MixName, string>(
        mixName => mixName.Value,
        value => MixName.Create(value));