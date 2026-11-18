using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;

using Beatport2Rss.Builder.Application.ReadModels.Feeds;
using Beatport2Rss.Builder.Application.ReadModels.Releases;

namespace Beatport2Rss.Builder.Application.Services.Feeds;

public sealed class RssFeedGenerator
{
    private const string AtomNamespace = "http://www.w3.org/2005/Atom";

    public string Generate(
        FeedReadModel feed,
        IEnumerable<ReleaseReadModel> releases)
    {
        ArgumentNullException.ThrowIfNull(feed);
        ArgumentNullException.ThrowIfNull(releases);

        using var stream = new MemoryStream();
        using (var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            Indent = true,
        }))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("rss");
            writer.WriteAttributeString("xmlns", "a10", null, AtomNamespace);
            writer.WriteAttributeString("version", "2.0");
            writer.WriteStartElement("channel");
            writer.WriteElementString("title", feed.Name.Value);
            writer.WriteElementString(
                "description",
                feed.AuthorName is { } authorName
                    ? $"Beatport releases feed built by {authorName.Value}"
                    : "Beatport releases feed");
            if (feed.AuthorName is { } managingEditor)
            {
                writer.WriteElementString("managingEditor", managingEditor.Value);
            }

            WriteAtomElement(writer, "id", feed.Id.Value.ToString("D"));

            foreach (var release in releases)
            {
                WriteItem(writer, feed.Id.Value, release);
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteItem(XmlWriter writer, Guid feedId, ReleaseReadModel release)
    {
        var artist = GetArtistName(release.Artists);
        writer.WriteStartElement("item");
        writer.WriteStartElement("guid");
        writer.WriteAttributeString("isPermaLink", "false");
        writer.WriteString($"{feedId:D}/{release.Id.Value:D}");
        writer.WriteEndElement();
        writer.WriteElementString("title", $"{artist} - {release.Name.Value}");
        writer.WriteElementString("description", GetDescription(release, artist));
        writer.WriteElementString("pubDate", release.CreatedAt.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture));
        WriteAtomLink(writer, $"https://www.beatport.com/release/{release.BeatportSlug.Value}/{release.BeatportId.Value}");
        writer.WriteElementString("image", release.ImageUri.AbsoluteUri);

        foreach (var category in release.Categories)
        {
            writer.WriteElementString("category", category);
        }

        writer.WriteEndElement();
    }

    private static string GetDescription(ReleaseReadModel release, string artist)
    {
        var builder = new StringBuilder();
        builder.AppendLine(CultureInfo.InvariantCulture, $"<img alt=\"{artist} - {release.Name.Value}\" src=\"{release.ImageUri.AbsoluteUri}\">");
        builder.AppendLine("<div style=\"font-family: sans-serif;\">");
        builder.AppendLine("<p>");
        builder.AppendLine(CultureInfo.InvariantCulture, $"<b>Artists:</b> {string.Join(", ", release.Artists.Select(FormatContributor))}<br>");
        builder.AppendLine(CultureInfo.InvariantCulture, $"<b>Name:</b> {release.Name.Value}<br>");
        builder.AppendLine(CultureInfo.InvariantCulture, $"<b>Label:</b> {FormatContributor(release.Label)}<br>");
        builder.AppendLine(CultureInfo.InvariantCulture, $"<b>Catalog:</b> {release.CatalogNumber.Value}<br>");
        builder.AppendLine(CultureInfo.InvariantCulture, $"<b>Released:</b> {release.ReleaseDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)}<br>");
        builder.AppendLine("</p>");
        builder.AppendLine("<p><b>Tracklist:</b><br>");

        var appendArtist = release.Artists.Any(artist =>
            release.Tracks.Any(track => track.Artists.All(trackArtist => trackArtist.SubscriptionId != artist.SubscriptionId)));

        foreach (var track in release.Tracks.OrderBy(track => track.Number))
        {
            builder.Append(CultureInfo.InvariantCulture, $"{track.Number:D2} ");

            if (appendArtist)
            {
                builder.Append(CultureInfo.InvariantCulture, $"{GetTrackArtistName(track.Artists)} - ");
            }

            builder.Append(CultureInfo.InvariantCulture, $"{track.Name} ({track.MixName}) ");

            if (appendArtist)
            {
                builder.Append(string.Join(", ", track.Artists.SelectMany(trackArtist => trackArtist.Tags.Select(tag => tag.Value))));
            }

            builder.Append(' ');
            builder.Append(CultureInfo.InvariantCulture, $"{FormatLength(track.Length)}<br>");
        }

        builder.AppendLine("</p>");

        var searchArtist = string.Join(
            " ",
            artist.Split(", ", StringSplitOptions.RemoveEmptyEntries)
                .Where(name => !string.Equals(name, "VA", StringComparison.OrdinalIgnoreCase)));
        var searchQuery = WebUtility.UrlEncode($"{searchArtist} {release.Name.Value}".Trim());
        builder.AppendLine(CultureInfo.InvariantCulture, $"<a href=\"https://music.youtube.com/search?q={searchQuery}\">Search on YouTube Music</a>");
        builder.Append("</div>");

        return builder.ToString();
    }

    private static string FormatContributor(ContributorReadModel contributor)
    {
        var tags = string.Join(", ", contributor.Tags.Select(tag => tag.Value));
        return string.IsNullOrEmpty(tags) ? contributor.Name.Value : $"{contributor.Name.Value} {tags}";
    }

    private static string GetArtistName(IReadOnlyCollection<ContributorReadModel> artists)
    {
        var names = GetDistinctArtistNames(artists);

        return names.Length > 5 ? "VA" : string.Join(", ", names);
    }

    private static string GetTrackArtistName(IReadOnlyCollection<ContributorReadModel> artists) =>
        string.Join(", ", GetDistinctArtistNames(artists));

    private static string[] GetDistinctArtistNames(IReadOnlyCollection<ContributorReadModel> artists) =>
        artists.Select(artist => artist.Name.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static string FormatLength(TimeSpan length) =>
        length.Hours == 0
            ? length.ToString(@"mm\:ss", CultureInfo.InvariantCulture)
            : length.ToString("c", CultureInfo.InvariantCulture);

    private static void WriteAtomElement(XmlWriter writer, string name, string value)
    {
        writer.WriteStartElement("a10", name, AtomNamespace);
        writer.WriteString(value);
        writer.WriteEndElement();
    }

    private static void WriteAtomLink(XmlWriter writer, string href)
    {
        writer.WriteStartElement("a10", "link", AtomNamespace);
        writer.WriteAttributeString("href", href);
        writer.WriteEndElement();
    }
}
