using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Security;

public interface IPasswordHasher
{
    PasswordHash Hash(Password password);

    bool Verify(Password password, PasswordHash hash);
}