#pragma warning disable CA1508 // Avoid dead conditional code

using Beatport2Rss.Interceptor.Interfaces;
using Beatport2Rss.Interceptor.Options;

using Microsoft.Extensions.Options;

using PuppeteerSharp;

namespace Beatport2Rss.Interceptor.Services;

internal sealed class BeatportAccessTokenInterceptor(
    ILogger<BeatportAccessTokenInterceptor> logger,
    IOptions<BeatportCredentials> beatportCredentials,
    IChromiumDownloader chromiumDownloader) :
    IBeatportAccessTokenInterceptor
{
    private readonly BeatportCredentials _beatportCredentials = beatportCredentials.Value;

    public async Task<(string AccessToken, int ExpiresIn)> InterceptAsync(
        bool headless,
        CancellationToken cancellationToken = default)
    {
        BeatportAccessTokenInterceptorLogMessages.Started(logger);

        var executablePath = await chromiumDownloader.EnsureInstalledAsync(cancellationToken);

        string? accessToken = null;
        int expiresIn = 0;

        var launchOptions = new LaunchOptions
        {
            ExecutablePath = executablePath,
            Headless = headless,
            Browser = SupportedBrowser.Chrome,
            Args = ["--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage"]
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

        BeatportAccessTokenInterceptorLogMessages.Intercepted(logger, accessToken, expiresIn);

        return string.IsNullOrEmpty(accessToken) || expiresIn == 0
            ? throw new InvalidOperationException("Either access token is empty or expires in is zero")
            : (accessToken, expiresIn);
    }
}

internal static partial class BeatportAccessTokenInterceptorLogMessages
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Started")]
    public static partial void Started(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Access token: {AccessToken}\nExpires in: {ExpiresIn}")]
    public static partial void Intercepted(ILogger logger, string? accessToken, int expiresIn);
}