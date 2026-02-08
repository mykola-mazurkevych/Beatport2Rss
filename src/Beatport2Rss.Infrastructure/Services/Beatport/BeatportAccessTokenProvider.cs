using System.IO.Compression;

using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Infrastructure.Options;

using Microsoft.Extensions.Options;

using PuppeteerSharp;

namespace Beatport2Rss.Infrastructure.Services.Beatport;

file static class ChromiumConstants
{
    public const string DirectoryName = "chrome-win";
    public const string ExecutableFileName = "chrome.exe";
    public const string UriString = "https://download-chromium.appspot.com/dl/Win";
}

internal sealed class BeatportAccessTokenProvider(
    IOptions<BeatportCredentials> beatportCredentials) :
    IBeatportAccessTokenProvider, IDisposable, IAsyncDisposable
{
    private readonly BeatportCredentials _beatportCredentials = beatportCredentials.Value;

    private readonly HttpClient _httpClient = new();

    public async Task<(string? AccessToken, int ExpiresIn)> ProvideAsync(CancellationToken cancellationToken = default)
    {
        string? accessToken = null;
        int expiresIn = 0;

        var tempPath = Path.GetTempPath();
        var directoryPath = $"{tempPath}{ChromiumConstants.DirectoryName}";
        var executablePath = $"{directoryPath}\\{ChromiumConstants.ExecutableFileName}";

        if (!File.Exists(executablePath))
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }

            await using var chromiumZipArchiveStream = await _httpClient.GetStreamAsync(new Uri(ChromiumConstants.UriString), cancellationToken).ConfigureAwait(false);
            await using var chromiumZipArchive = new ZipArchive(chromiumZipArchiveStream);
            await chromiumZipArchive.ExtractToDirectoryAsync(tempPath, cancellationToken);
        }

#if DEBUG
        const bool headless = false;
#else
        const bool headless = true;
#endif

        var launchOptions = new LaunchOptions { ExecutablePath = executablePath, Headless = headless, Browser = SupportedBrowser.Chrome };
        await using var browser = await Puppeteer.LaunchAsync(launchOptions).ConfigureAwait(false);

        await using var page = await browser.NewPageAsync().ConfigureAwait(false);
        await page.SetJavaScriptEnabledAsync(true).ConfigureAwait(false);

        page.Response += async (_, eventArgs) =>
        {
            var uri = new Uri(eventArgs.Response.Url);
            if (uri.Segments.All(segment => !string.Equals(segment, "token/", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var json = await eventArgs.Response.JsonAsync().ConfigureAwait(false);
            accessToken = json.RootElement.GetProperty("access_token").GetString();
            expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();
        };

        var popupTriggered = false;

        page.Popup += async (_, eventArgs) =>
        {
            if (popupTriggered)
            {
                return;
            }

            popupTriggered = true;

            await eventArgs.PopupPage.WaitForSelectorAsync("button[type='submit']").ConfigureAwait(false);

            await eventArgs.PopupPage.TypeAsync("#username", _beatportCredentials.Username).ConfigureAwait(false);
            await eventArgs.PopupPage.TypeAsync("#password", _beatportCredentials.Password).ConfigureAwait(false);
            await eventArgs.PopupPage.ClickAsync("button[type='submit']").ConfigureAwait(false);
        };

        await page.GoToAsync("https://api.beatport.com/v4/docs/").ConfigureAwait(false);
        await page.WaitForSelectorAsync(".Authenticator__button-text").ConfigureAwait(false);

        await page.ClickAsync(".Authenticator__button-text").ConfigureAwait(false);
        ////await page.WaitForSelectorAsync("div.Docs__navigation-container").ConfigureAwait(false);
        ////await page.WaitForResponseAsync("https://api.beatport.com/v4/swagger-ui/").ConfigureAwait(false);
        await page.WaitForResponseAsync("https://api.beatport.com/v4/swagger-ui/json/").ConfigureAwait(false);

        return (accessToken, expiresIn);
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