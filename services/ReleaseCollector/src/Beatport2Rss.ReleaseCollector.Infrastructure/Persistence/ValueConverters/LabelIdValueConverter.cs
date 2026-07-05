using Beatport2Rss.ReleaseCollector.Domain.Labels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.ValueConverters;

internal sealed class LabelIdValueConverter() :
    ValueConverter<LabelId, Guid>(
        labelId => labelId.Value,
        value => LabelId.Create(value));