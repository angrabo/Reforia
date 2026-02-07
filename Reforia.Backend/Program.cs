using ReforiaBackend.Extensions;
using ReforiaBackend.Hubs;

namespace ReforiaBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddApplicationServices();
        builder.Services.AddRpcFunctions();
        builder.Services.AddSignalR();
        
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
}