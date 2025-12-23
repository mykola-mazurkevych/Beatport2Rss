using Beatport2Rss.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.ValueConverters;

internal sealed class UserIdValueConverter() : ValueConverter<UserId, Guid>(userId => userId.Value, value => UserId.Create(value));