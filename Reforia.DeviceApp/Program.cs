using Reforia.IrcModule;
using Reforia.Rpc;
using ReforiaBackend.Extensions;
using ReforiaBackend.Hubs;
using Serilog;
using Serilog.Events;
using TestModule;

namespace ReforiaBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);
        var logPath = Path.Combine(logDirectory, "reforia.log"); // TODO: move log path to configuration.

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate:
                             "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day,outputTemplate:
                          "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}" )
            .CreateLogger();

        try
        {
            Log.Information("Starting Device App");

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

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

            app.Lifetime.ApplicationStarted.Register(() => Log.Information("Device App started"));
            app.Lifetime.ApplicationStopping.Register(() => Log.Information("Device App stopping"));
            app.Lifetime.ApplicationStopped.Register(() => Log.Information("Device App stopped"));

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            app.UseCors("desktop");

            app.UseAuthorization();

            app.MapHub<AppHub>("/hub");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Reforia backend terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}