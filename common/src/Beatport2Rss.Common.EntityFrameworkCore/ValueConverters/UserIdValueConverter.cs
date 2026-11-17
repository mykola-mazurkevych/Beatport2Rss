using Beatport2Rss.Common.Domain.Users;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.ValueConverters;

internal sealed class UserIdValueConverter() :
    ValueConverter<UserId, Guid>(
        userId => userId.Value,
        value => UserId.Create(value));