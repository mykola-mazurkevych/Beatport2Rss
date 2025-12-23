using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class PasswordHashValueConverter() : ValueConverter<PasswordHash, string>(passwordHash => passwordHash.Value, value => PasswordHash.Create(value));