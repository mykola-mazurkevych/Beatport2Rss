using Beatport2Rss.Builder.Application;
using Beatport2Rss.Builder.Infrastructure;
using Beatport2Rss.Builder.Jobs;
using Beatport2Rss.Common.Messaging;
using Beatport2Rss.Common.Miscellaneous;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddMessaging(builder.Configuration)
    .AddMiscellaneous()
    .AddJobs(builder.Configuration);

await builder.Build().RunAsync();