using Beatport2Rss.Api.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class EmailAddressValueConverter() :
    ValueConverter<EmailAddress, string>(
        emailAddress => emailAddress.Value,
        value => EmailAddress.Create(value));