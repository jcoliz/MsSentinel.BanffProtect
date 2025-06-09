using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Caching.Distributed;
using MsSentinel.BanffProtect.Application;
using MsSentinel.BanffProtect.Application.Fakes;
using MsSentinel.BanffProtect.Backend.Logs;
using MsSentinel.BanffProtect.Backend.Metrics;
using MsSentinel.BanffProtect.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "MsSentinel.BanffProtect.Backend";
    options.Description = "Application boundary between .NET backend and frontend.";
});

// Tune the JSON formatting
// https://www.meziantou.net/configuring-json-options-in-asp-net-core.htm
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = false;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddSingleton<BackEndMetrics>();
builder.Services.AddSingleton<IDistributedCache,FakeDistributedCache>();
builder.Services.AddSingleton<ConfigurationFeature>();
builder.Services.AddHostedService<SendLogsWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Add swagger UI
app.UseOpenApi();
app.UseSwaggerUi();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapPut("/config", async (ConnectorConfiguration config, ConfigurationFeature feature) =>
{
    app.Logger.LogInformation("Received config: {config}", JsonSerializer.Serialize(config));

    await feature.StoreAsync(config);
    return Results.NoContent();
})
.WithName("StoreConfig")
.Produces(204);

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
