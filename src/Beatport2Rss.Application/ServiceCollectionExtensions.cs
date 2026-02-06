#pragma warning disable CA1034 // Nested types should not be visible

using System.Reflection;

using Beatport2Rss.Application.Behaviors;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication() =>
            services
                .AddValidators()
                ////.AddValidatorsFromAssembly(typeof(ValidationBehavior<,>).Assembly)
                .AddMediator(o =>
                {
                    o.GenerateTypesAsInternal = true;
                    o.ServiceLifetime = ServiceLifetime.Transient;
                    o.PipelineBehaviors =
                    [
                        typeof(ValidationBehavior<,>),
                        typeof(UserValidationBehavior<,>),
                        typeof(FeedValidationBehavior<,>),
                    ];
                });

        private IServiceCollection AddValidators()
        {
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t is { IsAbstract: false, BaseType.IsGenericType: true } &&
                    t.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>))
                .ToList();

            types.ForEach(t => services.AddTransient(typeof(IValidator), t));

            return services;
        }
    }
}