using Microsoft.Extensions.DependencyInjection;

namespace HydraTentacle.Blazor.Infrastructure
{
    public static class AppServiceExtensions
    {
        public static IServiceCollection AddTentacleDependencies(this IServiceCollection services)
        {
            // Tentacle-specific servisler
            services.AddScoped<Services.RequestClient>();

            return services;
        }
    }
}
