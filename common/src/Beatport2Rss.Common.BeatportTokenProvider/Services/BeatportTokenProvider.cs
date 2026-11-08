using System.Net;

using Beatport2Rss.Common.BeatportTokenProvider.Services.Interfaces;
using Beatport2Rss.Common.FluentResults.Errors;
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
                HttpStatusCode.RequestTimeout => Result.Fail<string>(new RequestTimeoutError(errorMessage)),
                HttpStatusCode.TooManyRequests => Result.Fail<string>(new TooManyRequestsError(errorMessage)),
                { } statusCode when (int)statusCode is >= 500 and <= 599 => Result.Fail<string>(new TransientHttpError(errorMessage, statusCode)),
                null => Result.Transient<string>(errorMessage),
                _ => Result.Fail<string>(errorMessage),
            };
        }
    }
}