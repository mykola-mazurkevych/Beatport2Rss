using Beatport2Rss.TokenProvider.Options;
using Beatport2Rss.TokenProvider.Services.Interfaces;

using Microsoft.Extensions.Options;

using PuppeteerSharp;

namespace Beatport2Rss.TokenProvider.Services;

internal sealed class BeatportAccessTokenInterceptor(
    IOptions<BeatportCredentials> beatportCredentials,
    IChromiumDownloader chromiumDownloader) :
    IBeatportAccessTokenInterceptor
{
    private readonly BeatportCredentials _beatportCredentials = beatportCredentials.Value;

    public async Task<(string? AccessToken, int ExpiresIn)> InterceptAsync(
        bool headless,
        CancellationToken cancellationToken = default)
    {
        var executablePath = await chromiumDownloader.EnsureInstalledAsync(cancellationToken);

        string? accessToken = null;
        int expiresIn = 0;

        var launchOptions = new LaunchOptions
        {
            ExecutablePath = executablePath,
            Headless = headless,
            Browser = SupportedBrowser.Chrome,
        };
        await using var browser = await Puppeteer.LaunchAsync(launchOptions);

        await using var page = await browser.NewPageAsync();
        await page.SetJavaScriptEnabledAsync(true);

        page.Response += async (_, eventArgs) =>
        {
            var uri = new Uri(eventArgs.Response.Url);
            if (uri.Segments.All(segment => !string.Equals(segment, "token/", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var json = await eventArgs.Response.JsonAsync();
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

            await eventArgs.PopupPage.WaitForSelectorAsync("button[type='submit']");

            await eventArgs.PopupPage.TypeAsync("#username", _beatportCredentials.Username);
            await eventArgs.PopupPage.TypeAsync("#password", _beatportCredentials.Password);
            await eventArgs.PopupPage.ClickAsync("button[type='submit']");
        };

        await page.GoToAsync("https://api.beatport.com/v4/docs/");
        await page.WaitForSelectorAsync(".Authenticator__button-text");

        await page.ClickAsync(".Authenticator__button-text");
        ////await page.WaitForSelectorAsync("div.Docs__navigation-container");
        ////await page.WaitForResponseAsync("https://api.beatport.com/v4/swagger-ui/");
        await page.WaitForResponseAsync("https://api.beatport.com/v4/swagger-ui/json/");

        return (accessToken, expiresIn);
    }
}