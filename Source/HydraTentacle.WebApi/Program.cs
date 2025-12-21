using HydraTentacle.WebApi.Extensions;
using HydraTentacle.Core.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Hydra.Services.Core;
using HydraTentacle.Core.Models;
using Hydra.AccessManagement;
using Hydra.WebApi.Extensions;
using Hydra.WebApi.Middlewares;

namespace HydraTentacle.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //test
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register Controllers from Hydra.WebApi
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(MainController<>).Assembly);

            // Add Tentacle Dependencies
            builder.Services.AddTentacleDependencies(builder.Configuration);
            
            // Add Hydra Dependencies Explicitly
            builder.Services.AddHydraDependencies(builder.Configuration, typeof(HydraTentacle.Core.Models.Request.Request).Assembly);

            builder.Services.AddDbContext<TentacleDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost:5120", "https://localhost:7238")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });



            var app = builder.Build();

            // Resolve LogService for Startup Logging
            using (var scope = app.Services.CreateScope())
            {
                var logService = scope.ServiceProvider.GetRequiredService<Hydra.Services.ILogService>();
                
                logService.SaveAsync(Hydra.Core.LogFactory.Info("Startup", "Init", "Hydra Tentacle Starting..."), Hydra.Services.LogRecordType.Console).Wait();
                logService.SaveAsync(Hydra.Core.LogFactory.Info("Startup", "Init", "Services Registered"), Hydra.Services.LogRecordType.Console).Wait();
            }

            // Initialize Databases (Log & Main)
            Hydra.Services.DbInitializer.InitializeAsync<TentacleDbContext>(app.Services, app.Configuration);

            // Configure the HTTP request pipeline.

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();


            app.UseCors();

            app.UseMiddleware<SessionMiddleware>();


            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Datetime : {DateTime.Now} Request: {context.Request.Method} {context.Request.Path}");

                await next.Invoke(); // pipeline devam eder

                Console.WriteLine($"Datetime : {DateTime.Now} Response: {context.Response.StatusCode} for {context.Request.Path}");
            });

            // Middleware ile IP veya Origin kontrolü
            app.Use(async (context, next) =>
            {
                var origin = context.Request.Headers["Origin"].ToString();
                var remoteIp = context.Connection.RemoteIpAddress?.ToString();

                // Sadece izinli origin veya IP
                var allowedOrigins = new[] { "https://localhost:7238", "http://localhost:5120" };
                var allowedIps = new[] { "127.0.0.1", "::1" };

                if (!allowedOrigins.Contains(origin) && !allowedIps.Contains(remoteIp))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden");
                    return;
                }

                await next();
            });


            app.UseAuthorization();


            app.MapControllers();
            
            using (var scope = app.Services.CreateScope())
            {
                var logService = scope.ServiceProvider.GetRequiredService<Hydra.Services.ILogService>();
                logService.SaveAsync(Hydra.Core.LogFactory.Info("Startup", "Init", $"Application Running at: {string.Join(", ", app.Urls)}"), Hydra.Services.LogRecordType.Console).Wait();
            }

            app.Run();
        }
    }
}
