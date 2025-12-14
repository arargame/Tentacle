
namespace HydraTentacle.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //test
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();


            app.UseCors();


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

            app.Run();
        }
    }
}
