using HydraTentacle.Blazor.Core.Views.RequestViews;

namespace HydraTentacle.Blazor.Core.DI
{
    public static class ServiceRegistration
    {
        public static void AddViews(this IServiceCollection services)
        {
            services.AddScoped<ListViewForRequest>();
        }
    }
}
