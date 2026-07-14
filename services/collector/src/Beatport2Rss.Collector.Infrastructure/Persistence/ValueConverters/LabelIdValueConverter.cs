using Beatport2Rss.Collector.Domain.Labels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Collector.Infrastructure.Persistence.ValueConverters;

internal sealed class LabelIdValueConverter() :
    ValueConverter<LabelId, Guid>(
        labelId => labelId.Value,
        value => LabelId.Create(value));