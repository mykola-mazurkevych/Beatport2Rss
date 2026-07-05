using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.ReadModels.Users;

public sealed record UserStatusDetailsReadModel(
    UserStatus Status);