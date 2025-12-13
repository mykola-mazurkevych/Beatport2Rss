using Beatport2Rss.Contracts.Interfaces;

namespace Beatport2Rss.Infrastructure.Security;

internal sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}