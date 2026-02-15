using Reforia.IrcModule;
using Reforia.Rpc;
using ReforiaBackend.Extensions;
using ReforiaBackend.Hubs;
using TestModule;

namespace ReforiaBackend;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddSignalR(options => { options.EnableDetailedErrors = true; });

            builder.Services.AddLogging();

            builder.Services.AddReforiaBackend();

            builder.Services.AddRpc()
                .AddTourneyModule()
                .AddIrcModule();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("desktop", policy =>
                {
                    policy.WithOrigins("http://localhost:1420", "https://localhost:1420")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("desktop");

            app.UseAuthorization();

            app.MapHub<AppHub>("/hub");

            app.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}