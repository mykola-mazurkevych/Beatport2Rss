using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;
using Beatport2Rss.Common.IntegrationEvents.Users;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Resend;

using Wolverine;

using DeliveryConfirmationOptions = Beatport2Rss.Api.Infrastructure.Options.EmailConfirmationOptions;

namespace Beatport2Rss.Api.Infrastructure.Extensions.EmailConfirmations;

public static class ConfirmationEmailDeliveryExtensions
{
    public static IServiceCollection AddConfirmationEmailDelivery(this IServiceCollection services)
    {
        services.AddOptions<ResendClientOptions>()
            .Configure<IOptions<DeliveryConfirmationOptions>>(static (client, delivery) => client.ApiToken = delivery.Value.ApiKey);

        services.AddHttpClient<IResend, ResendClient>()
            .ConfigurePrimaryHttpMessageHandler(static () => new HttpClientHandler { AllowAutoRedirect = false })
            .RemoveAllLoggers();

        return services
            .AddTransient<ResendConfirmationEmail>()
            .AddScoped<IConfirmationEmailOutbox, WolverineConfirmationEmailOutbox>();
    }

    public static WolverineOptions ConfigureConfirmationEmailDelivery(this WolverineOptions options)
    {
        // Explicit registration keeps the durable delivery contract internal without relying on handler code generation.
        options.AddMessageHandler(new SendConfirmationEmailHandler());
        options.Discovery.IncludeType<UserCreatedConfirmationHandler>();
        options.LocalQueue("user-created-confirmation").Sequential().UseDurableInbox();
        options.PublishMessage<UserCreated>().ToLocalQueue("user-created-confirmation").UseDurableInbox();
        options.PublishMessage<SendConfirmationEmail>()
            .ToLocalQueue("confirmation-email-delivery")
            .UseDurableInbox();
        return options;
    }
}
