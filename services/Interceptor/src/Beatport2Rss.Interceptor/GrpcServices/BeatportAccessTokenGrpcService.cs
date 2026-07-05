using Beatport2Rss.Interceptor.Interfaces;

using BeatportAccessTokenService;

using Grpc.Core;

namespace Beatport2Rss.Interceptor.GrpcServices;

internal sealed class BeatportAccessTokenGrpcService(
    IAccessTokenProvider accessTokenProvider) :
    GrpcBeatportAccessTokenService.GrpcBeatportAccessTokenServiceBase
{
    public override async Task<GetTokenResponse> GetToken(
        GetTokenRequest request,
        ServerCallContext context)
    {
        var accessToken = await accessTokenProvider.ProvideAsync(context.CancellationToken);

        return new GetTokenResponse { AccessToken = accessToken };
    }
}