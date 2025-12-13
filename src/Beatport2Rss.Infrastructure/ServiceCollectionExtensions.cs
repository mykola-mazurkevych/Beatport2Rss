#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Contracts.Interfaces;
using Beatport2Rss.Infrastructure.Security;
using Beatport2Rss.Infrastructure.Utilities;

using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Infrastructure;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure() =>
            services
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>()
                .AddSingleton<ISlugGenerator, SlugGenerator>();
    }
}