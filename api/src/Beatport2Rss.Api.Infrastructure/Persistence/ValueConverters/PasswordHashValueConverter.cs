using Beatport2Rss.Api.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Api.Infrastructure.Persistence.ValueConverters;

internal sealed class PasswordHashValueConverter() :
    ValueConverter<PasswordHash, string>(
        passwordHash => passwordHash.Value,
        value => PasswordHash.Create(value));