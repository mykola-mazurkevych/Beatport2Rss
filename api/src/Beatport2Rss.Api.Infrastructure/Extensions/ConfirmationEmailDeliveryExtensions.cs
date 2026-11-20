using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Resend;

using Wolverine;

namespace Beatport2Rss.Api.Infrastructure.Extensions;

public static class ConfirmationEmailDeliveryExtensions
{
    public static IServiceCollection AddConfirmationEmailDelivery(this IServiceCollection services)
    {
        services.AddOptions<ResendClientOptions>()
            .Configure<IOptions<EmailConfirmationOptions>>(static (client, delivery) => client.ApiToken = delivery.Value.ApiKey);

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
        options.PublishMessage<SendConfirmationEmail>()
            .ToLocalQueue("confirmation-email-delivery")
            .UseDurableInbox();
        return options;
    }
}
