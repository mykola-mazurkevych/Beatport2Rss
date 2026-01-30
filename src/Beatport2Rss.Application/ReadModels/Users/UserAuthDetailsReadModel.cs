using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.ReadModels.Users;

public sealed record UserAuthDetailsReadModel(
    UserId Id,
    EmailAddress EmailAddress,
    PasswordHash PasswordHash,
    string? FirstName,
    string? LastName);