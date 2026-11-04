using Beatport2Rss.Common.Domain.Tags;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class TagNameValueConverter() :
    ValueConverter<TagName, string>(
        tagName => tagName.Value,
        value => TagName.Create(value));