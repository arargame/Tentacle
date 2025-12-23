using Hydra.RazorClassLibrary.Utils;
using HydraTentacle.Blazor.Components;
using HydraTentacle.Blazor.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server;

namespace HydraTentacle.Blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //test
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Register Hydra Razor Library Services (Auth, Http, Storage)
            builder.Services.AddHydraRazorLibrary();

            // Register Tentacle-specific services
            builder.Services.AddTentacleDependencies();

            builder.Services.AddScoped(sp =>
            {
                return new HttpClient { BaseAddress = new Uri("http://localhost:5132/api/") };
            });

            builder.Services.AddServerSideBlazor()
                            .AddCircuitOptions(options => { options.DetailedErrors = true; });


            // Legacy Service - Temporarily kept until Views are migrated
            // Legacy Service - Temporarily kept until Views are migrated
            // HydraTentacle.Blazor.Core.DI.ServiceRegistration.AddViews(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
