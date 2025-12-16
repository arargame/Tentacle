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
using HydraTentacle.Core.BusinessLayer.Services;

namespace HydraTentacle.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTentacleDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // First, add standard Hydra dependencies
            services.AddHydraDependencies(configuration);

            // Define which assemblies to scan for Attributes (Only scan Tentacle specific ones, Hydra ones are handled by AddHydraDependencies)
            var assembliesToScan = new[]
            {
                // Tentacle Core Assembly (for Request, RequestService etc)
                typeof(Request).Assembly,

                // Current WebApi Assembly (if there are any specific services here)
                Assembly.GetExecutingAssembly()
            };

            // Register Services
            services.AddCustomServicesByAttribute<RegisterAsServiceAttribute>(assembliesToScan);

            // Register Repositories
            services.AddCustomServicesByAttribute<RegisterAsRepositoryAttribute>(assembliesToScan);

            // Load ViewDTOs (if Tentacle has specific views, though ViewDTORegistryLoader in Hydra might handle generic + executing)
            // But we explicitly load Tentacle assembly views just in case
            ViewDTORegistryLoader.LoadAllViewDTOs(typeof(Request).Assembly);

            // Register DbContext for RepositoryInjector
            // Register DbContext for RepositoryInjector
            services.AddScoped<DbContext>(provider => provider.GetRequiredService<TentacleDbContext>());
            
            // Explicitly register Core Hydra Services by concrete type
            services.AddScoped<RoleSystemUserService>();
            services.AddScoped<RoleService>();
            services.AddScoped<SystemUserService>();
            services.AddScoped<RolePermissionService>();
            services.AddScoped<SystemUserPermissionService>();
            services.AddScoped<PermissionService>();
            services.AddScoped<IService<Request>, RequestService>(); // Explicit registration
            services.AddScoped<LogService>(); // Register concrete LogService
            services.AddScoped<IHttpContextAccessorAbstraction, HttpContextAccessorWrapper>();
            
            services.AddScoped<RepositoryInjector>();
            services.AddScoped<FileService>();

            // Override RepositoryFactoryService to include Tentacle assemblies
            services.AddScoped<IRepositoryFactoryService>(sp =>
            {
                return new RepositoryFactoryService(
                    sp,
                    sp.GetRequiredService<LogService>(),
                    assembliesToScan
                );
            });

            return services;
        }
    }
}
