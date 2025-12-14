using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Contracts.Interfaces;

public interface IPasswordHasher
{
    PasswordHash Hash(Password password);

    bool Verify(Password password, PasswordHash hash);
}