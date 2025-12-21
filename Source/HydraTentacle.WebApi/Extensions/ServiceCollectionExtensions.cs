using Hydra.Services;
using Hydra.DI;
using Hydra.Core;
using Hydra.DAL.Core;
using HydraTentacle.Core.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Hydra.DTOs;
using Hydra.DTOs.ViewDTOs;
using Hydra.Services.Core;
using Hydra.WebApi.Extensions;
using HydraTentacle.Core.Models;
using System.Reflection;
using Hydra.DI.HttpContextDI;
using HydraTentacle.Core.BusinessLayer.Services.Request;
using RequestModel = HydraTentacle.Core.Models.Request.Request;

namespace HydraTentacle.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTentacleDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // First, add standard Hydra dependencies
            // services.AddHydraDependencies(configuration); // Removed to be explicit in Program.cs

            // Define which assemblies to scan for Attributes (Only scan Tentacle specific ones, Hydra ones are handled by AddHydraDependencies)
            var assembliesToScan = new[]
            {
                // Tentacle Core Assembly (for Request, RequestService etc)
                typeof(RequestModel).Assembly,

                // Current WebApi Assembly (if there are any specific services here)
                Assembly.GetExecutingAssembly()
            };

            // Services and Repositories are registered by AddHydraDependencies in Program.cs passing the assembly.
            // keeping this clean to avoid Duplication Exception.

            // Load ViewDTOs (if Tentacle has specific views, though ViewDTORegistryLoader in Hydra might handle generic + executing)
            // But we explicitly load Tentacle assembly views just in case
            ViewDTORegistryLoader.LoadAllViewDTOs(typeof(RequestModel).Assembly);

            // Register DbContext for RepositoryInjector
            // Register DbContext for RepositoryInjector
            services.AddScoped<DbContext>(provider => provider.GetRequiredService<TentacleDbContext>());
            
            // Concrete services are registered automatically by AddHydraDependencies -> AddBusinessLayerDependencies scanning Hydra.Services assembly.
            // services.AddScoped<RoleSystemUserService>();
            // services.AddScoped<RoleService>();
            // services.AddScoped<SystemUserService>();
            // services.AddScoped<RolePermissionService>();
            // services.AddScoped<SystemUserPermissionService>();
            // services.AddScoped<PermissionService>();
            // services.AddScoped<IService<RequestModel>, RequestService>(); // Handled by Attribute scan in AddHydraDependencies
            //services.AddScoped<LogService>(); // Register concrete LogService
            services.AddScoped<IHttpContextAccessorAbstraction, HttpContextAccessorWrapper>();
            
            services.AddScoped<RepositoryInjector>();
            services.AddScoped<FileService>();



            return services;
        }
    }
}
