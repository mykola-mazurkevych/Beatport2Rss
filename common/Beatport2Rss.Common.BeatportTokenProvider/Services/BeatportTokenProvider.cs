using Beatport2Rss.Common.BeatportTokenProvider.Services.Interfaces;

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

            return Result.Fail($"Token service call failed: {message}");
        }
        catch (HttpRequestException exception)
        {
            return Result.Fail($"Token service request failed: {exception.Message}");
        }
    }
}