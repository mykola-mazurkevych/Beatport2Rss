using System.IO.Compression;

using Beatport2Rss.Application.Interfaces.Services.Misc;

namespace Beatport2Rss.Infrastructure.Services.Misc;

internal sealed class ChromiumDownloader :
    IChromiumDownloader, IDisposable, IAsyncDisposable
{
    private const string DirectoryName = "chrome-win";
    private const string ExecutableFileName = "chrome.exe";
    private const string UriString = "https://download-chromium.appspot.com/dl/Win";

    private readonly HttpClient _httpClient = new();

    public async Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default)
    {
        var tempPath = Path.GetTempPath();
        var directoryPath = $"{tempPath}{DirectoryName}";
        var executablePath = $"{directoryPath}\\{ExecutableFileName}";

        if (!File.Exists(executablePath))
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }

            await using var chromiumZipArchiveStream = await _httpClient.GetStreamAsync(new Uri(UriString), cancellationToken).ConfigureAwait(false);
            await using var chromiumZipArchive = new ZipArchive(chromiumZipArchiveStream);
            await chromiumZipArchive.ExtractToDirectoryAsync(tempPath, cancellationToken);
        }

        return executablePath;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        _httpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}