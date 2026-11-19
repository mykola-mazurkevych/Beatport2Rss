using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public static class EmailConfirmationIdentifier
{
    public const int HashLength = 64;

    public static string Generate() => Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));

    public static string? Hash(string identifier)
    {
        if (identifier.Length != 43 || identifier.Any(static c => !char.IsAsciiLetterOrDigit(c) && c is not '-' and not '_'))
        {
            return null;
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.ASCII.GetBytes(identifier)));
    }
}
