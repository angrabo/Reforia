using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Reforia.Core.Common.Database;
using Reforia.Core.Utils;
using ReforiaBackend.Extensions;
using ReforiaBackend.Hubs;
using ReforiaBackend.Utils;
using ReforiaBackend.Utils.Static;
using Serilog;
using Serilog.Events;

namespace ReforiaBackend;

public class Program
{
    public static void Main(string[] args)
    {
        ConfigureBootstrapper();

        try
        {
            Logger.Info("Starting Device App");
            var builder = WebApplication.CreateBuilder(args);

            ConfigureHost(builder);
            ConfigureServices(builder);

            var app = builder.Build();

            ConfigureMiddleware(app);
            InitializeLifecycleLogs(app);
            
            if (!File.Exists(Paths.DatabasePath))
            {
                InitializeDatabase(app);
            }

            app.Run();
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Reforia backend terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureBootstrapper()
    {
        var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File(Paths.LogFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, outputTemplate: outputTemplate)
            .CreateLogger();
    }

    private static void ConfigureHost(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog();
        builder.WebHost.UseUrls("http://localhost:5727");
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
     
        builder.Services.AddSignalR(options => { options.EnableDetailedErrors = true; })
            .AddJsonProtocol(options => {
                options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddReforiaBackend(new ServicesOptionsModel
        {
            DatabseConnectionString = $"Data Source={Paths.DatabasePath}"
        });

        // CORS - zintegrowany
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", policy =>
            {
                var origins = builder.Environment.IsDevelopment() 
                    ? new[] { "http://localhost:1420", "https://localhost:1420" }
                    : new[] { "http://tauri.localhost" };

                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        Logger.Init(app.Services.GetRequiredService<ILoggerFactory>());
        
        app.UseHttpsRedirection(); 
        
        app.UseCors("DefaultPolicy");
        
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<AppHub>("/hub");
    }

    private static void InitializeLifecycleLogs(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() => Log.Information("Device App started"));
        app.Lifetime.ApplicationStopped.Register(() => Log.Information("Device App stopped"));
    }

    private static void InitializeDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
            Logger.Info("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occurred while migrating the database.");
        }
    }
}