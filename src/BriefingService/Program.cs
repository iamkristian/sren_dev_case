using Microsoft.EntityFrameworkCore;
using BriefingService.Data;
using Microsoft.Extensions.Logging;
using Prometheus;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Extensions.Hosting;

public class Program 
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Setup db connections
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configure logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    // Configure OpenTelemetry
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BriefingService"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter();
        })
        .WithMetrics(meterProviderBuilder =>
        {
            meterProviderBuilder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BriefingService"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddPrometheusExporter();
        });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseHttpMetrics(); // Enable Prometheus metrics
    app.MapControllers();
    app.UseMetricServer(); // Start the Prometheus metrics server
    app.Run();
  }
}
