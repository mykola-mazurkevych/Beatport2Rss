using System.IO.Compression;

using Beatport2Rss.TokenProvider.Services.Interfaces;

namespace Beatport2Rss.TokenProvider.Services;

internal sealed class ChromiumDownloader(HttpClient httpClient) :
    IChromiumDownloader
{
    private const string DirectoryName = "chromium";
    private const string ExecutableFileName = "chrome.exe";
    private const string UriString = "https://download-chromium.appspot.com/dl/Win";

    public async Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default)
    {
        var tempPath = Path.GetTempPath();
        var directoryPath = $"{tempPath}{DirectoryName}";
        var executablePath = $"{directoryPath}\\{ExecutableFileName}";

        if (File.Exists(executablePath))
        {
            return executablePath;
        }

        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, recursive: true);
        }

        await using var zipArchiveStream = await httpClient.GetStreamAsync(new Uri(UriString), cancellationToken);
        await using var zipArchive = new ZipArchive(zipArchiveStream);
        await zipArchive.ExtractToDirectoryAsync(tempPath, cancellationToken);

        return executablePath;
    }
}