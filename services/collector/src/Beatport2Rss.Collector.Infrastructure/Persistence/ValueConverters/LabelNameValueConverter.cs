using Beatport2Rss.Collector.Domain.Labels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class LabelNameValueConverter() :
    ValueConverter<LabelName, string>(
        labelName => labelName.Value,
        value => LabelName.Create(value));