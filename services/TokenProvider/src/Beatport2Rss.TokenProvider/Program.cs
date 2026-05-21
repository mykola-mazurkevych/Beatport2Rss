using Beatport2Rss.TokenProvider.Options;
using Beatport2Rss.TokenProvider.GrpcServices;
using Beatport2Rss.TokenProvider.Services;
using Beatport2Rss.TokenProvider.Services.Interfaces;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services
    .Configure<BeatportCredentials>(credentials => builder.Configuration.GetSection(nameof(BeatportCredentials)).Bind(credentials))
    .Configure<ChromiumDownloaderOptions>(options => builder.Configuration.GetSection(nameof(ChromiumDownloaderOptions)).Bind(options));

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IChromiumDownloader, ChromiumDownloader>((provider, httpClient) =>
{
    var options = provider.GetRequiredService<IOptions<ChromiumDownloaderOptions>>().Value;
    httpClient.BaseAddress = options.BaseAddress;
});

builder.Services
    .AddSingleton<IAccessTokenProvider, AccessTokenProvider>()
    .AddSingleton<IBeatportAccessTokenInterceptor, BeatportAccessTokenInterceptor>()
    .AddSingleton<IChromiumDownloader, ChromiumDownloader>();

var app = builder.Build();

app.MapGrpcService<TokenGrpcService>();

app.Run();