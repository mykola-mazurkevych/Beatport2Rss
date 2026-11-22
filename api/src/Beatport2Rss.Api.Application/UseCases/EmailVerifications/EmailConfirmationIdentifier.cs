using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

using Beatport2Rss.Api.Domain.EmailVerifications;

namespace Beatport2Rss.Api.Application.UseCases.EmailVerifications;

public static class EmailConfirmationIdentifier
{
    public static string Generate() => Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));

    public static SecretHash? Hash(string? identifier)
    {
        if (identifier is null ||
            identifier.Length != 43 ||
            identifier.Any(static c => !char.IsAsciiLetterOrDigit(c) && c is not '-' and not '_'))
        {
            return null;
        }

        return SecretHash.Create(Convert.ToHexString(SHA256.HashData(Encoding.ASCII.GetBytes(identifier))));
    }
}
