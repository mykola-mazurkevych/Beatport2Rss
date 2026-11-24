using Beatport2Rss.Api.Domain.EmailConfirmations;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class EmailConfirmationIdValueConverter() :
    ValueConverter<EmailConfirmationId, Guid>(
        emailConfirmationId => emailConfirmationId.Value,
        value => EmailConfirmationId.Create(value));