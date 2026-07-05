using Beatport2Rss.ReleaseCollector.Domain.Labels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class LabelNameValueConverter() :
    ValueConverter<LabelName, string>(
        labelName => labelName.Value,
        value => LabelName.Create(value));