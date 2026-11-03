using Beatport2Rss.Common.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class BeatportSlugValueConverter() :
    ValueConverter<BeatportSlug, string>(
        beatportSlug => beatportSlug.Value,
        value => BeatportSlug.Create(value));