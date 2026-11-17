using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Feeds.Queries;
using Beatport2Rss.Common.Domain.Common;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using Wolverine;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Feeds.Queries;

public sealed class GetRssFeedQueriesTests
{
    private static readonly DateTimeOffset GeneratedAt = new(2026, 11, 17, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task GeneratedAtQuery_ShouldReturnTimestamp()
    {
        var token = TestContext.Current.CancellationToken;
        const string slug = "my-feed";
        var slugValue = Slug.Create(slug);
        var repository = new Mock<IRssFeedDocumentQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetGeneratedAtAsync(slugValue, token))
            .ReturnsAsync(GeneratedAt);

        var result = await new GetRssFeedGeneratedAtQueryHandler(repository.Object)
            .Handle(new GetRssFeedGeneratedAtQuery(slugValue.Value), token);

        Assert.Equal(GeneratedAt, result);
    }

    [Fact]
    public async Task ContentQuery_ShouldReturnContent()
    {
        var token = TestContext.Current.CancellationToken;
        const string slug = "my-feed";
        var slugValue = Slug.Create(slug);
        const string content = "<rss />";
        var repository = new Mock<IRssFeedDocumentQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetContentAsync(slugValue, token))
            .ReturnsAsync(content);

        var result = await new GetRssFeedDocumentContentQueryHandler(repository.Object)
            .HandleAsync(new GetRssFeedDocumentContentQuery(slugValue.Value), token);

        Assert.Equal(content, result);
    }

    [Fact]
    public async Task InvokedThroughWolverine_ShouldHandleBothQueries()
    {
        var token = TestContext.Current.CancellationToken;
        const string slug = "my-feed";
        var slugValue = Slug.Create(slug);
        const string content = "<rss />";
        var repository = new Mock<IRssFeedDocumentQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetGeneratedAtAsync(slugValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GeneratedAt);
        repository.Setup(value => value.GetContentAsync(slugValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);
        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(repository.Object);
                services.AddWolverine(options =>
                    options.Discovery.IncludeAssembly(typeof(GetRssFeedGeneratedAtQuery).Assembly));
            })
            .Build();
        await host.StartAsync(token);

        var messageBus = host.Services.GetRequiredService<IMessageBus>();
        var generatedAt = await messageBus
            .InvokeAsync<DateTimeOffset>(new GetRssFeedGeneratedAtQuery(slug), token);
        var returnedContent = await messageBus
            .InvokeAsync<string>(new GetRssFeedDocumentContentQuery(slug), token);
        await host.StopAsync(token);

        Assert.Equal(GeneratedAt, generatedAt);
        Assert.Equal("<rss />", returnedContent);
    }
}