using Beatport2Rss.Api.Application.Interfaces.Models;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.ReadModels.Users;

public sealed record UserAuthDetailsReadModel(
    UserId Id,
    EmailAddress EmailAddress,
    PasswordHash PasswordHash,
    string? FirstName,
    string? LastName) :
    IUserAuthDetails;