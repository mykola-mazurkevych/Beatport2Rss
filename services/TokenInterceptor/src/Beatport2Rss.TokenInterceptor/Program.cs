using Beatport2Rss.TokenInterceptor.GrpcServices;
using Beatport2Rss.TokenInterceptor.HostedServices;
using Beatport2Rss.TokenInterceptor.Interfaces;
using Beatport2Rss.TokenInterceptor.Options;
using Beatport2Rss.TokenInterceptor.Services;

using Microsoft.Extensions.Options;

using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddHealthChecks();

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
    .AddSingleton<IChromiumDownloader, ChromiumDownloader>()
    .AddResiliencePipeline(
        AccessTokenProvider.ResiliencePipelineKey, 
        pipeline =>
        {
            pipeline.AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 });
            pipeline.AddTimeout(TimeSpan.FromMinutes(5));
        })
    .AddHostedService<ChromiumWarmup>();

var app = builder.Build();

app.MapGrpcService<BeatportAccessTokenGrpcService>();
app.MapGrpcReflectionService();
app.MapHealthChecks("/health");

app.Run();