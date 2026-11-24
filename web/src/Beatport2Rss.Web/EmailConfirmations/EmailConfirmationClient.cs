using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Beatport2Rss.Web.EmailConfirmations;

internal sealed class EmailConfirmationClient(HttpClient httpClient)
{
    public Task<ConfirmationState> GetStateAsync(string token) => SendAsync(HttpMethod.Get, token);

    public Task<ConfirmationState> ConfirmAsync(string token) => SendAsync(HttpMethod.Put, token);

    private async Task<ConfirmationState> SendAsync(HttpMethod method, string token)
    {
        using var request = new HttpRequestMessage(method, $"email-confirmations/{Uri.EscapeDataString(token)}");
        request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true, NoStore = true };
        if (method == HttpMethod.Put)
        {
            request.Content = JsonContent.Create(new { confirmed = true });
        }

        try
        {
            using var response = await httpClient.SendAsync(request);
            if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            {
                return ConfirmationState.Invalid;
            }

            if (method == HttpMethod.Put && response.StatusCode == HttpStatusCode.NoContent)
            {
                return ConfirmationState.Confirmed;
            }

            if (method == HttpMethod.Get && response.StatusCode == HttpStatusCode.OK)
            {
                var resource = await response.Content.ReadFromJsonAsync<ConfirmationResource>();
                return (resource?.IsConfirmed, resource?.IsExpired) switch
                {
                    (true, _) => ConfirmationState.Confirmed,
                    (false, true) => ConfirmationState.Invalid,
                    (false, false) => ConfirmationState.Ready,
                    _ => ConfirmationState.Unavailable
                };
            }
        }
        catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException or JsonException)
        {
            // Neither upstream error content nor exceptions containing the secret URL belong in the UI or logs.
        }

        return ConfirmationState.Unavailable;
    }

    private sealed record ConfirmationResource(bool? IsConfirmed, bool? IsExpired);
}
