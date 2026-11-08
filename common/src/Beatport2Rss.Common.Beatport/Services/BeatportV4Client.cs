using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.FluentResults.Extensions;

using FluentResults;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.Beatport.Services;

internal sealed class BeatportV4Client(
    HttpClient httpClient,
    IOptions<JsonSerializerOptions> jsonSerializerOptions) :
    IBeatportClient
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;

    public async Task<Result<TBeatportDto?>> GetAsync<TBeatportDto>(
        int id,
        string accessToken,
        CancellationToken cancellationToken = default)
        where TBeatportDto : BeatportDto
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{GetSegment<TBeatportDto>()}/{id}/", UriKind.Relative));
        request.Version = HttpVersion.Version11;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Beatport2Rss", "1.0"));

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var beatportDto = await response.Content.ReadFromJsonAsync<TBeatportDto>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
            return beatportDto;
        }

        var status = $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim();
        var contentAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var message = TryGetDetail(contentAsString, out var detail)
            ? $"{status}: {detail}"
            : status;

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => Result.Unauthorized(message),
            HttpStatusCode.Forbidden => Result.Forbidden(message),
            HttpStatusCode.NotFound => Result.NotFound(message),
            HttpStatusCode.RequestTimeout => Result.RequestTimeout(message),
            HttpStatusCode.TooManyRequests => Result.TooManyRequests(message),
            _ when (int)response.StatusCode is >= 500 and <= 599 => Result.Transient(message, response.StatusCode),
            _ => Result.Fail(message),
        };
    }

    private bool TryGetDetail(string body, [NotNullWhen(true)] out string? detail)
    {
        detail = null;

        if (string.IsNullOrWhiteSpace(body))
        {
            return false;
        }

        try
        {
            detail = JsonSerializer.Deserialize<DetailedResult>(body, _jsonSerializerOptions)?.Detail;
            return !string.IsNullOrEmpty(detail);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static string GetSegment<TBeatportDto>() =>
        typeof(TBeatportDto).Name switch
        {
            nameof(BeatportArtistDto) => "artists",
            nameof(BeatportLabelDto) => "labels",
            _ => throw new InvalidOperationException($"Beatport dto type '{nameof(TBeatportDto)}' is not supported")
        };

    private sealed record DetailedResult(string? Detail);
}