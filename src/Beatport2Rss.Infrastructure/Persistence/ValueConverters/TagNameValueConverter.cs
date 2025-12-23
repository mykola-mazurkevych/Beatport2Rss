using Beatport2Rss.Domain.Tags;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class TagNameValueConverter() : ValueConverter<TagName, string>(tagName => tagName.Value, value => TagName.Create(value));