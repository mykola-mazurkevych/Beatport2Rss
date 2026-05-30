using System.IO.Compression;

using Beatport2Rss.TokenProvider.Options;
using Beatport2Rss.TokenProvider.Services.Interfaces;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.TokenProvider.Services;

internal sealed class ChromiumDownloader(
    ILogger<ChromiumDownloader> logger,
    HttpClient httpClient,
    IOptions<ChromiumDownloaderOptions> options) :
    IChromiumDownloader
{
    private const string DefaultDirectoryName = "chromium";
    private readonly ChromiumDownloaderOptions _options = options.Value;

    public async Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default)
    {
        var cacheDirectory = GetCacheDirectory();
        var executableName = GetExecutableName();
        var executablePath = Path.Combine(cacheDirectory, executableName);

        if (File.Exists(executablePath))
        {
            ChromiumDownloaderLogMessages.ExecutablePath(logger, executablePath);
            return executablePath;
        }

        if (Directory.Exists(cacheDirectory))
        {
            Directory.Delete(cacheDirectory, recursive: true);
        }

        Directory.CreateDirectory(cacheDirectory);

        var platformFolder = GetPlatformFolder();
        var downloadUri = new Uri(_options.BaseAddress, $"dl/{platformFolder}");

        ChromiumDownloaderLogMessages.Downloading(logger, downloadUri);

        await using var zipArchiveStream = await httpClient.GetStreamAsync(downloadUri, cancellationToken);
        await using var zipArchive = new ZipArchive(zipArchiveStream);
        await zipArchive.ExtractToDirectoryAsync(cacheDirectory, cancellationToken);

        var actualExecutablePath = Directory
            .EnumerateFiles(cacheDirectory, executableName, SearchOption.AllDirectories)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(actualExecutablePath))
        {
            throw new InvalidOperationException("Chromium executable not found after extraction.");
        }

        ChromiumDownloaderLogMessages.ExecutablePath(logger, actualExecutablePath);

        return actualExecutablePath;
    }

    private string GetCacheDirectory() =>
        string.IsNullOrWhiteSpace(_options.CacheDirectory)
            ? Path.Combine(Path.GetTempPath(), DefaultDirectoryName)
            : _options.CacheDirectory;

    private static string GetPlatformFolder() =>
        OperatingSystem.IsWindows() ? "Win" :
        OperatingSystem.IsLinux() ? "Linux_x64" :
        OperatingSystem.IsMacOS() ? "Mac" :
        throw new PlatformNotSupportedException();

    private static string GetExecutableName() =>
        OperatingSystem.IsWindows() ? "chrome.exe" : "chrome";
}

internal static partial class ChromiumDownloaderLogMessages
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Chromium location: {ExecutablePath}")]
    public static partial void ExecutablePath(ILogger logger, string executablePath);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Downloading Chromium from {Uri}")]
    public static partial void Downloading(ILogger logger, Uri uri);
}