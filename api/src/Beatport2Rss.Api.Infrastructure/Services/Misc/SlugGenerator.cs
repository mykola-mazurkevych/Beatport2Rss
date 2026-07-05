using System.Security.Cryptography;

using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Domain.Common.ValueObjects;

using Slugify;

namespace Beatport2Rss.Api.Infrastructure.Services.Misc;

internal sealed class SlugGenerator(ISlugHelper slugHelper) :
    ISlugGenerator
{
    private const string AlphaNumericChars = "abcdefghijklmnopqrstuvwxyz0123456789";

    public Slug Generate(string inputString)
    {
        var prefix = slugHelper.GenerateSlug(inputString);
        var suffix = GenerateSuffix();

        var slug = Slug.Create($"{prefix}{Slug.Delimiter}{suffix}");

        return slug;
    }

    private static string GenerateSuffix()
    {
        var chars = new char[Slug.SuffixLength];

        for (var i = 0; i < Slug.SuffixLength; i++)
        {
            chars[i] = AlphaNumericChars[RandomNumberGenerator.GetInt32(AlphaNumericChars.Length)];
        }

        return new string(chars);
    }
}