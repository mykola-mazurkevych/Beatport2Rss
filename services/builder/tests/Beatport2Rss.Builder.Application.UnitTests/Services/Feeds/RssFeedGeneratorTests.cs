using System.Xml.Linq;

using Beatport2Rss.Builder.Application.ReadModels.Releases;
using Beatport2Rss.Builder.Application.ReadModels.Feeds;
using Beatport2Rss.Builder.Application.Services.Feeds;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.Services.Feeds;

public sealed class RssFeedGeneratorTests
{
    private static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";

    [Fact]
    public void Generate_ShouldGenerateRssItemsWithLegacyFieldsAndEscapedHtmlDescription()
    {
        var feedId = Guid.NewGuid();
        var releaseId = Guid.NewGuid();
        var artistId = Guid.NewGuid();
        var release = new ReleaseReadModel(
            ReleaseId.Create(releaseId),
            BeatportId.Create(123),
            BeatportSlug.Create("some-release"),
            new DateTimeOffset(2026, 11, 13, 17, 31, 30, TimeSpan.Zero),
            ReleaseName.Create("Some & Kind Of Feeling"),
            new Uri("https://example.com/image.jpg"),
            CatalogNumber.Create("ENCOLOR751E"),
            new DateOnly(2026, 11, 13),
            new ContributorReadModel(SubscriptionId.Create(Guid.NewGuid()), SubscriptionName.Create("Colorize"), [TagName.Create("[UK]")]),
            [new ContributorReadModel(SubscriptionId.Create(artistId), SubscriptionName.Create("Lipless"), [TagName.Create("[Australia]")])],
            [
                new TrackReadModel(
                    1,
                    TrackName.Create("Some Kind Of Feeling"),
                    MixName.Create("Extended Mix"),
                    new TimeSpan(0, 5, 7),
                    [new ContributorReadModel(SubscriptionId.Create(artistId), SubscriptionName.Create("Lipless"), [])]),
            ],
            ["Deep"]);

        var feed = new FeedReadModel(
            FeedId.Create(feedId),
            FeedName.Create("Deep & Melodic"),
            AuthorName.Create("mmazur"));
        var xml = new RssFeedGenerator().Generate(feed, [release]);
        var document = XDocument.Parse(xml);
        var channel = document.Root!.Element("channel")!;
        var item = channel.Element("item")!;

        Assert.Equal("2.0", document.Root.Attribute("version")!.Value);
        Assert.Equal("Deep & Melodic", channel.Element("title")!.Value);
        Assert.Equal(feedId.ToString("D"), channel.Element(Atom + "id")!.Value);
        Assert.Equal($"{feedId:D}/{releaseId:D}", item.Element("guid")!.Value);
        Assert.Equal("false", item.Element("guid")!.Attribute("isPermaLink")!.Value);
        Assert.Equal("Lipless - Some & Kind Of Feeling", item.Element("title")!.Value);
        Assert.Equal("Deep", item.Element("category")!.Value);
        Assert.Equal("https://www.beatport.com/release/some-release/123", item.Element(Atom + "link")!.Attribute("href")!.Value);
        Assert.Contains("<b>Artists:</b> Lipless [Australia]<br>", item.Element("description")!.Value);
        Assert.Contains("<b>Released:</b> 13-11-2026<br>", item.Element("description")!.Value);
        Assert.Contains("01 Some Kind Of Feeling (Extended Mix)", item.Element("description")!.Value);
        Assert.Contains("05:07<br>", item.Element("description")!.Value);
        Assert.Contains("Some & Kind Of Feeling", item.Element("description")!.Value);
    }

    [Fact]
    public void Generate_WhenReleaseArtistIsMissingFromTrack_ShouldIncludeTrackArtist()
    {
        var releaseArtistId = Guid.NewGuid();
        var release = new ReleaseReadModel(
            ReleaseId.Create(Guid.NewGuid()),
            BeatportId.Create(123),
            BeatportSlug.Create("release"),
            DateTimeOffset.UtcNow,
            ReleaseName.Create("Release"),
            new Uri("https://example.com/image.jpg"),
            CatalogNumber.Create("CAT"),
            new DateOnly(2026, 1, 1),
            new ContributorReadModel(SubscriptionId.Create(Guid.NewGuid()), SubscriptionName.Create("Label"), []),
            [new ContributorReadModel(SubscriptionId.Create(releaseArtistId), SubscriptionName.Create("Release Artist"), [])],
            [
                new TrackReadModel(
                    1,
                    TrackName.Create("Track"),
                    MixName.Create("Original Mix"),
                    TimeSpan.FromMinutes(3),
                    [new ContributorReadModel(SubscriptionId.Create(Guid.NewGuid()), SubscriptionName.Create("Track Artist"), [])]),
            ],
            []);

        var feed = new FeedReadModel(
            FeedId.Create(Guid.NewGuid()),
            FeedName.Create("Feed"),
            AuthorName.Create("author"));
        var item = XDocument.Parse(new RssFeedGenerator().Generate(feed, [release]))
            .Descendants("item")
            .Single();

        Assert.Contains("01 Track Artist - Track (Original Mix)", item.Element("description")!.Value);
        Assert.Contains("03:00<br>", item.Element("description")!.Value);
    }
}
