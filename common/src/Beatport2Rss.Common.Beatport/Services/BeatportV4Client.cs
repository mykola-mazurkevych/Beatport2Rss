using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.FluentResults.Extensions;

using FluentResults;

using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.Beatport.Services;

internal sealed class BeatportV4Client(
    HttpClient httpClient,
    IBeatportTokenProvider beatportTokenProvider,
    IOptions<JsonSerializerOptions> jsonSerializerOptions) :
    IBeatportClient
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;

    public async Task<Result<TBeatportDto?>> GetAsync<TBeatportDto>(
        int id,
        CancellationToken cancellationToken = default)
        where TBeatportDto : BeatportDto
    {
        var httpRequestMessageResult = await InitializeHttpRequestMessageAsync($"{GetSegment<TBeatportDto>()}/{id}/", cancellationToken);
        if (httpRequestMessageResult.IsFailed)
        {
            return Result.Fail(httpRequestMessageResult);
        }

        using var request = httpRequestMessageResult.Value;
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var beatportDto = await HandleResponseAsync<TBeatportDto>(response, cancellationToken);

        return beatportDto;
    }

    public async Task<Result<IReadOnlyCollection<int>>> SearchForIdsAsync<TBeatportDto>(
        IReadOnlyDictionary<string, string?> queryParameters,
        CancellationToken cancellationToken = default)
        where TBeatportDto : BeatportDto
    {
        List<int> ids = [];

        var parameters = new Dictionary<string, string?>(queryParameters);

        var page = 1;

        do
        {
            parameters[nameof(page)] = page.ToString(CultureInfo.InvariantCulture);

            var query = string.Join(
                separator: "&",
                values: parameters.Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value ?? string.Empty)}"));
            var httpRequestMessageResult = await InitializeHttpRequestMessageAsync($"{GetSegment<TBeatportDto>()}/?{query}", cancellationToken);
            if (httpRequestMessageResult.IsFailed)
            {
                return Result.Fail(httpRequestMessageResult);
            }

            using var request = httpRequestMessageResult.Value;
            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var pagedResult = await HandleResponseAsync<BeatportPagedResponse>(response, cancellationToken);
            if (pagedResult.IsFailed)
            {
                return Result.Fail(pagedResult);
            }

            var pagedResponse = pagedResult.Value!;

            ids.AddRange(pagedResponse.Results.Select(result => result.Id));

            if (pagedResponse.Next is null)
            {
                break;
            }

            page++;
        } while (true);

        return ids.AsReadOnly();
    }

    private static string GetSegment<TBeatportDto>() =>
        typeof(TBeatportDto).Name switch
        {
            nameof(BeatportArtistDto) => "artists",
            nameof(BeatportLabelDto) => "labels",
            _ => throw new InvalidOperationException($"Beatport dto type '{nameof(TBeatportDto)}' is not supported")
        };

    private async Task<Result<HttpRequestMessage>> InitializeHttpRequestMessageAsync(
        string uriString,
        CancellationToken cancellationToken)
    {
        var requestUri = new Uri(uriString, UriKind.Relative);
        var accessTokenResult = await beatportTokenProvider.ProvideAsync(cancellationToken).ConfigureAwait(false);
        if (accessTokenResult.IsFailed)
        {
            return Result.Fail(accessTokenResult);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Version = HttpVersion.Version11;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenResult.Value);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Beatport2Rss", "1.0"));

        return request;
    }

    private async Task<Result<TDto?>> HandleResponseAsync<TDto>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.Content.ReadFromJsonAsync<TDto>(_jsonSerializerOptions, cancellationToken);
        }

        var errorMessage = await GetErrorMessageAsync(response, cancellationToken);

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => Result.Unauthorized(errorMessage),
            HttpStatusCode.Forbidden => Result.Forbidden(errorMessage),
            HttpStatusCode.NotFound => Result.NotFound(errorMessage),
            HttpStatusCode.RequestTimeout => Result.RequestTimeout(errorMessage),
            HttpStatusCode.TooManyRequests => Result.TooManyRequests(errorMessage),
            _ when (int)response.StatusCode is >= 500 and <= 599 => Result.Transient(errorMessage, response.StatusCode),
            _ => Result.Fail(errorMessage),
        };
    }

    private async Task<string> GetErrorMessageAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var status = $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim();
        var contentAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        string? detail = null;
        try
        {
            detail = JsonSerializer.Deserialize<BeatportDetailedResponse>(contentAsString, _jsonSerializerOptions)?.Detail;
        }
        catch (JsonException)
        {
        }

        var errorMessage = string.IsNullOrEmpty(detail)
            ? status
            : $"{status}: {detail}";

        return errorMessage;
    }

    // ReSharper disable ClassNeverInstantiated.Local

    private sealed record BeatportDetailedResponse(
        [property: JsonPropertyName("detail")] string? Detail);

    private sealed record IdDto(
        [property: JsonPropertyName("id")] int Id);

    private sealed record BeatportPagedResponse(
        [property: JsonPropertyName("results")] IReadOnlyCollection<IdDto> Results,
        [property: JsonPropertyName("next")] string? Next);
}