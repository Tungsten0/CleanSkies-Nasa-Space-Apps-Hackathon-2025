using Microsoft.Extensions.Caching.Memory;
using CleanSkies.Models;
using CleanSkies.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

// Register our custom services
builder.Services.AddHttpClient<IOpenAQApi, OpenAQApi>();

builder.Services.Configure<OpenAQSettings>(
    builder.Configuration.GetSection("OpenAQApi")
);

var app = builder.Build();

// Static files + Razor
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// ---------------- Minimal API endpoints ---------------- //

//If we want to cache it

// Nearby stations (OpenAQ + basic AQI mapping)
// app.MapGet("/api/location", async (
//     double lat, double lon,
//     IOpenAQApi openAQ,
//     IMemoryCache cache) =>
// {
//     var cacheKey = $"nearby_{lat:F4}_{lon:F4}";
//     if (!cache.TryGetValue(cacheKey, out var result))
//     {
//         result = await openAQ.GetLocationDataAsync(2158);
//         cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
//     }
//     return Results.Ok(result);
// });

app.MapGet("/api/location/{locationId}", async (
    int locationId,
    IOpenAQApi openAQ,
    ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation($"API request for location {locationId}");
        var result = await openAQ.GetLocationDataAsync(locationId);
        return Results.Content(result, "application/json");
    }
    catch (HttpRequestException ex)
    {
        logger.LogError(ex, "HTTP Request failed");
        return Results.Json(new
        {
            error = "API request failed",
            message = ex.Message,
            locationId = locationId
        }, statusCode: 500);
    }
});

// app.MapGet("/api/aq/latest", (double? lat, double? lon, IMemoryCache cache) =>
// {
//     const string CacheKey = "aq:latest";        // 👈 agree on this key with your teammate

//     if (!cache.TryGetValue(CacheKey, out LatestAqResponse? payload))
//     {
//         // Seed temporary fake so frontend can proceed
//         payload = BuildFake(lat ?? 42.30, lon ?? -83.00);
//         cache.Set(CacheKey, payload, TimeSpan.FromMinutes(2)); // short TTL
//     }

//     return Results.Json(payload, new JsonSerializerOptions
//     {
//         PropertyNamingPolicy = JsonNamingPolicy.CamelCase // frontend gets camelCase
//     });
// });

app.Run();
