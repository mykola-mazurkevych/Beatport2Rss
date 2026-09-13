#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.Miscellaneous.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Common.Miscellaneous;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMiscellaneous() =>
            services.AddTransient<IClock, Clock>();
    }
}