using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class UsernameValueConverter() : ValueConverter<Username, string>(username => username.Value, value => Username.Create(value));