using Beatport2Rss.Collector.Application;
using Beatport2Rss.Collector.Infrastructure;
using Beatport2Rss.Collector.Worker;
using Beatport2Rss.Common.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddMessaging(builder.Configuration);

builder.Services.AddHostedService<SubscriptionCreatedRabbitMqWorker>();
builder.Services.AddHostedService<FeedSubscriptionCreatedRabbitMqWorker>();
builder.Services.AddHostedService<FeedSubscriptionDeletedRabbitMqWorker>();

await builder.Build().RunAsync();