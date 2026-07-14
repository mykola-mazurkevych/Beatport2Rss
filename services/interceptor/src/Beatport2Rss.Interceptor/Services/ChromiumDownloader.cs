using System.IO.Compression;

using Beatport2Rss.Interceptor.Interfaces;
using Beatport2Rss.Interceptor.Options;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Interceptor.Services;

internal sealed class ChromiumDownloader(
    ILogger<ChromiumDownloader> logger,
    HttpClient httpClient,
    IOptions<ChromiumDownloaderOptions> options) :
    IChromiumDownloader
{
    private readonly ChromiumDownloaderOptions _options = options.Value;

    public async Task<string> EnsureInstalledAsync(CancellationToken cancellationToken = default)
    {
        (string folderName, string executableName, string uriSegment) = GetPlatformSpecificOptions();

        var path = Path.Combine(Path.GetTempPath(), folderName);
        var executablePath = Path.Combine(path, executableName);

        ChromiumDownloaderLogMessages.ExecutablePath(logger, executablePath);

        if (File.Exists(executablePath))
        {
            return executablePath;
        }

        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }

        Directory.CreateDirectory(path);

        var downloadUri = new Uri(_options.BaseAddress, $"dl/{uriSegment}");

        ChromiumDownloaderLogMessages.Downloading(logger, downloadUri);

        await using var zipArchiveStream = await httpClient.GetStreamAsync(downloadUri, cancellationToken);
        await using var zipArchive = new ZipArchive(zipArchiveStream);
        await zipArchive.ExtractToDirectoryAsync(Path.GetTempPath(), cancellationToken);

        return File.Exists(executablePath)
            ? executablePath
            : throw new InvalidOperationException("Chromium executable not found after extraction.");
    }

    private static (string FolderName, string ExecutableName, string UriSegment) GetPlatformSpecificOptions() =>
        OperatingSystem.IsWindows() ? ("chrome-win", "chrome.exe", "Win_x64") :
        OperatingSystem.IsLinux() ? ("chrome-linux", "chrome", "Linux_x64") :
        throw new PlatformNotSupportedException();
}

internal static partial class ChromiumDownloaderLogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Chromium executable path: {ExecutablePath}")]
    public static partial void ExecutablePath(ILogger logger, string executablePath);

    [LoggerMessage(Level = LogLevel.Information, Message = "Downloading Chromium from {Uri}")]
    public static partial void Downloading(ILogger logger, Uri uri);
}