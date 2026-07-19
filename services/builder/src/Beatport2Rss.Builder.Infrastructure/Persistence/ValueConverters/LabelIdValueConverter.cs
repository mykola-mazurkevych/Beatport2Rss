using Beatport2Rss.Builder.Domain.Labels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class LabelIdValueConverter() :
    ValueConverter<LabelId, Guid>(
        labelId => labelId.Value,
        value => LabelId.Create(value));