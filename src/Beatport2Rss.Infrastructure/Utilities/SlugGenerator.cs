using System.Security.Cryptography;

using Beatport2Rss.Contracts.Interfaces;

using Slugify;

namespace Beatport2Rss.Infrastructure.Utilities;

public sealed class SlugGenerator(ISlugHelper slugHelper) : ISlugGenerator
{
    private const string AlphaNumericChars = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int SuffixLength = 4;

    public string Generate(string inputString)
    {
        var slug = slugHelper.GenerateSlug(inputString);
        var suffix = GenerateSuffix();

        return $"{slug}-{suffix}";
    }

    private static string GenerateSuffix()
    {
        var chars = new char[SuffixLength];

        for (var i = 0; i < SuffixLength; i++)
        {
            chars[i] = AlphaNumericChars[RandomNumberGenerator.GetInt32(AlphaNumericChars.Length)];
        }

        return new string(chars);
    }
}