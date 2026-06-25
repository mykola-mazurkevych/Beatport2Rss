using Beatport2Rss.TokenInterceptor.Services.Interfaces;

using BeatportAccessTokenService;

using Grpc.Core;

namespace Beatport2Rss.TokenInterceptor.GrpcServices;

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