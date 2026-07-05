using Beatport2Rss.Collector.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class MixNameValueConverter() :
    ValueConverter<MixName, string>(
        mixName => mixName.Value,
        value => MixName.Create(value));