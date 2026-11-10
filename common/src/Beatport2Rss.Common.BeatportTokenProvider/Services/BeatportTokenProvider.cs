using System.Net;

using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.FluentResults.Extensions;

using BeatportAccessTokenService;

using FluentResults;

using Grpc.Core;

namespace Beatport2Rss.Common.BeatportTokenProvider.Services;

internal sealed class BeatportTokenProvider(
    GrpcBeatportAccessTokenService.GrpcBeatportAccessTokenServiceClient client) :
    IBeatportTokenProvider
{
    public async Task<Result<string>> ProvideAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetTokenRequest();
            var response = await client.GetTokenAsync(request, cancellationToken: cancellationToken);
            return Result.Ok(response.AccessToken);
        }
        catch (RpcException exception)
        {
            var message = string.IsNullOrWhiteSpace(exception.Status.Detail)
                ? exception.Message
                : exception.Status.Detail;

            var errorMessage = $"Token service call failed: {message}";
            return exception.StatusCode switch
            {
                StatusCode.DeadlineExceeded or StatusCode.ResourceExhausted or StatusCode.Unavailable => Result.Transient<string>(errorMessage),
                _ => Result.Fail<string>(errorMessage),
            };
        }
        catch (HttpRequestException exception)
        {
            var errorMessage = $"Token service request failed: {exception.Message}";
            return exception.StatusCode switch
            {
                HttpStatusCode.RequestTimeout => Result.RequestTimeout(errorMessage),
                HttpStatusCode.TooManyRequests => Result.TooManyRequests(errorMessage),
                { } statusCode when (int)statusCode is >= 500 and <= 599 => Result.Transient(errorMessage, statusCode),
                null => Result.Transient<string>(errorMessage),
                _ => Result.Fail<string>(errorMessage),
            };
        }
    }
}