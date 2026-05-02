using Beatport2Rss.Domain.Tracks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class MixNameValueConverter() :
    ValueConverter<MixName, string>(
        mixName => mixName.Value,
        value => MixName.Create(value));