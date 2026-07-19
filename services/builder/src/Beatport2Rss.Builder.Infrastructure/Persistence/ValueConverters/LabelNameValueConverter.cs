using Beatport2Rss.Builder.Domain.Labels;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Builder.Infrastructure.Persistence.ValueConverters;

internal sealed class LabelNameValueConverter() :
    ValueConverter<LabelName, string>(
        labelName => labelName.Value,
        value => LabelName.Create(value));