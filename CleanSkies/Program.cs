using Microsoft.Extensions.Caching.Memory;
using CleanSkies.Models;
using CleanSkies.Services;

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

app.Run();
