using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Options;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.Beatport.Services;

internal sealed class BeatportUriBuilder(IOptions<BeatportOptions> beatportOptions) :
    IBeatportUriBuilder
{
    private readonly BeatportOptions _beatportOptions = beatportOptions.Value;

    public Uri Build(int id, string slug) =>
        new(_beatportOptions.WebBaseUri, $"{slug.TrimStart('/')}/{id}");
}