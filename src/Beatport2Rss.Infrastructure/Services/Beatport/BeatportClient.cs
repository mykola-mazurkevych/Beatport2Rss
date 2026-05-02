using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Beatport2Rss.Application.Dtos.Beatport;
using Beatport2Rss.Application.Interfaces.Services.Beatport;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tokens;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Infrastructure.Services.Beatport;

internal sealed class BeatportClient(
    IHttpClientFactory httpClientFactory,
    IOptions<JsonSerializerOptions> jsonSerializerOptions) :
    IBeatportClient
{
    private const string BeatportApiUriString = "https://api.beatport.com/v4/catalog";

    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<Result<TBeatportDto?>> GetAsync<TBeatportDto>(
        BeatportId id,
        BeatportAccessToken token,
        CancellationToken cancellationToken = default)
        where TBeatportDto : BeatportDto
    {
        var uriSegment = GetSegment<TBeatportDto>();
        var uri = new Uri($"{BeatportApiUriString}/{uriSegment}/{id}/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                var dto = await response.Content.ReadFromJsonAsync<TBeatportDto>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
                return dto;
            case HttpStatusCode.Forbidden:
                var forbiddenResult = await response.Content.ReadFromJsonAsync<ForbiddenResult>(_jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
                return Result.Forbidden(forbiddenResult?.Detail ?? "Forbidden.");
            default:
                return Result.Unprocessable($"Beatport API return {response.StatusCode} status code.");
        }
    }

    private static string GetSegment<TBeatportDto>() =>
        typeof(TBeatportDto).Name switch
        {
            nameof(BeatportArtistDto) => "artists",
            nameof(BeatportLabelDto) => "labels",
            _ => throw new InvalidOperationException($"Beatport dto type '{nameof(TBeatportDto)}' is not supported.")
        };

    private sealed record ForbiddenResult(string Detail);
}