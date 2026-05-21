using Beatport2Rss.TokenProvider.Services.Interfaces;

using Grpc.Core;

namespace Beatport2Rss.TokenProvider.GrpcServices;

internal sealed class TokenGrpcService(
    IAccessTokenProvider accessTokenProvider) :
    TokenService.TokenServiceBase
{
    public override async Task<TokenReply> GetToken(
        Empty request,
        ServerCallContext context)
    {
        var accessToken = await accessTokenProvider.ProvideAsync(context.CancellationToken);

        return new TokenReply { AccessToken = accessToken };
    }
}