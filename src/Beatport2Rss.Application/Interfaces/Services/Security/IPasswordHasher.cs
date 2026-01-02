using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Interfaces.Services.Security;

public interface IPasswordHasher
{
    PasswordHash Hash(Password password);

    bool Verify(Password password, PasswordHash hash);
}