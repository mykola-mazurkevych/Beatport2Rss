using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Infrastructure.Services;

internal sealed class BCryptPasswordHasher : IPasswordHasher
{
    public PasswordHash Hash(Password password) =>
        PasswordHash.Create(BCrypt.Net.BCrypt.HashPassword(password.Value));

    public bool Verify(Password password, PasswordHash hash) =>
        BCrypt.Net.BCrypt.Verify(password.Value, hash.Value);
}