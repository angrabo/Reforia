using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Reforia.Core.Common.Database;
using Reforia.Core.Utils;
using ReforiaBackend.Extensions;
using ReforiaBackend.Hubs;
using Serilog;
using Serilog.Events;

namespace ReforiaBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);
        var logPath = Path.Combine(logDirectory, "reforia.log"); // TODO: move log path to configuration.
        var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "reforia.db");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
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
            builder.Services.AddReforiaBackend(new ServicesOptionsModel()
            {
                DatabseConnectionString = $"Data Source={databasePath}"
            });
            builder.WebHost.UseUrls("http://localhost:5727");
            
            #if DEBUG
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("dev", policy =>
                {
                    policy.WithOrigins("http://localhost:1420", "https://localhost:1420")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            #endif
            
            #if !DEBUG
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("prod", policy =>
                {
                    policy.WithOrigins("http://tauri.localhost")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            #endif

            builder.Services.AddOpenApi();

            var app = builder.Build();
            
            app.Lifetime.ApplicationStarted.Register(() => Log.Information("Device App started"));
            app.Lifetime.ApplicationStopping.Register(() => Log.Information("Device App stopping"));
            app.Lifetime.ApplicationStopped.Register(() => Log.Information("Device App stopped"));


            Console.WriteLine(databasePath);
            
            if (!File.Exists(databasePath))
                InitializeDatabase(app);
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            #if DEBUG
            app.UseCors("dev");
            #endif
            
            #if !DEBUG
            app.UseCors("prod");
            #endif
            
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

    private static void InitializeDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while migrating the database.");
        }
    }
}