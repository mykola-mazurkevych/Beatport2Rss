using Beatport2Rss.Common.Domain.Common;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class EmailAddressValueConverter() :
    ValueConverter<EmailAddress, string>(
        emailAddress => emailAddress.Value,
        value => EmailAddress.Create(value));