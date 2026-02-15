using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.ReadModels.Users;

public sealed record UserStatusReadModel(
    UserStatus Status);