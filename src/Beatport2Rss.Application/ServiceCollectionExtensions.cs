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
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddMediator(o =>
                {
                    o.GenerateTypesAsInternal = true;
                    o.ServiceLifetime = ServiceLifetime.Transient;
                    ////o.Assemblies = [Assembly.GetExecutingAssembly()];
                    o.PipelineBehaviors =
                    [
                        typeof(ValidationBehavior<,>),
                        typeof(ForbiddenBehavior<,>),
                    ];
                });
    }
}