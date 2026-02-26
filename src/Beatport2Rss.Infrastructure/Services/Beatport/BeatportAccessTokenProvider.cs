using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Infrastructure.Options;

using Microsoft.Extensions.Options;

using PuppeteerSharp;

namespace Beatport2Rss.Infrastructure.Services.Beatport;

internal sealed class BeatportAccessTokenProvider(
    IOptions<BeatportCredentials> beatportCredentials,
    IChromiumDownloader chromiumDownloader) :
    IBeatportAccessTokenProvider
{
#if DEBUG
    private const bool Headless = false;
#else
    private const bool Headless = true;
#endif

    private readonly BeatportCredentials _beatportCredentials = beatportCredentials.Value;

    public async Task<(string? AccessToken, int ExpiresIn)> ProvideAsync(CancellationToken cancellationToken = default)
    {
        var executablePath = await chromiumDownloader.EnsureInstalledAsync(cancellationToken);

        string? accessToken = null;
        int expiresIn = 0;

        var launchOptions = new LaunchOptions { ExecutablePath = executablePath, Headless = Headless, Browser = SupportedBrowser.Chrome };
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
}