using Beatport2Rss.Api.Application.Interfaces.Services.Security;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Infrastructure.Services.Security;

internal sealed class BCryptPasswordHasher :
    IPasswordHasher
{
    public PasswordHash Hash(Password password) =>
        PasswordHash.Create(BCrypt.Net.BCrypt.HashPassword(password.Value));

    public bool Verify(Password password, PasswordHash hash) =>
        BCrypt.Net.BCrypt.Verify(password.Value, hash.Value);
}