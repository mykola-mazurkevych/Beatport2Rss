using Beatport2Rss.Builder.Infrastructure;
using Beatport2Rss.Builder.Jobs;
using Beatport2Rss.Builder.Worker.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddHealthChecks();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddJobs(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<RssFeedGrpcService>();
app.MapGrpcReflectionService();
app.MapHealthChecks("/health");

await app.RunAsync();
