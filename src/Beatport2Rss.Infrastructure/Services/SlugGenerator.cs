using System.Security.Cryptography;

using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using Slugify;

namespace Beatport2Rss.Infrastructure.Services;

internal sealed class SlugGenerator(ISlugHelper slugHelper) : ISlugGenerator
{
    private const string AlphaNumericChars = "abcdefghijklmnopqrstuvwxyz0123456789";

    public Slug Generate(FeedName feedName) =>
        Slug.Create(Generate(feedName.Value));

    public Slug Generate(Username username) =>
        Slug.Create(Generate(username.Value));

    private string Generate(string inputString)
    {
        var slug = slugHelper.GenerateSlug(inputString);
        var suffix = GenerateSuffix();

        return $"{slug}{Slug.Delimiter}{suffix}";
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