using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;

using FluentResults;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.Beatport.Services;

internal sealed class BeatportClient(
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
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{GetSegment<TBeatportDto>()}/{id}", UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                var beatportDto = await response.Content.ReadFromJsonAsync<TBeatportDto>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
                return beatportDto;
            case HttpStatusCode.Forbidden:
                var forbiddenResult = await response.Content.ReadFromJsonAsync<ForbiddenResult>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
                return Result.Fail(forbiddenResult?.Detail ?? "Forbidden");
            default:
                return Result.Fail($"Beatport API return {response.StatusCode} status code");
        }
    }

    private static string GetSegment<TBeatportDto>() =>
        typeof(TBeatportDto).Name switch
        {
            nameof(BeatportArtistDto) => "artists",
            nameof(BeatportLabelDto) => "labels",
            _ => throw new InvalidOperationException($"Beatport dto type '{nameof(TBeatportDto)}' is not supported")
        };

    private sealed record ForbiddenResult(string Detail);
}