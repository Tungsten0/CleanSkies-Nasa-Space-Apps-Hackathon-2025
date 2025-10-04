using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

// Register our custom services
builder.Services.AddScoped<Services.OpenAQService>();
builder.Services.AddScoped<Services.MeteomaticsService>();
builder.Services.AddScoped<Services.TempoService>();

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

// Nearby stations (OpenAQ + basic AQI mapping)
app.MapGet("/api/nearby", async (
    double lat, double lon,
    Services.OpenAQService openAQ,
    IMemoryCache cache) =>
{
    var cacheKey = $"nearby_{lat:F4}_{lon:F4}";
    if (!cache.TryGetValue(cacheKey, out var result))
    {
        result = await openAQ.GetNearbyAsync(lat, lon);
        cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
    }
    return Results.Ok(result);
});

// Forecast (Meteomatics + simple baseline logic)
app.MapGet("/api/forecast", async (
    double lat, double lon,
    Services.MeteomaticsService meteo) =>
{
    var forecast = await meteo.GetForecastAsync(lat, lon);
    return Results.Ok(forecast);
});

// TEMPO (satellite overlay or cached sample)
app.MapGet("/api/tempo", async (
    double lat, double lon,
    Services.TempoService tempo) =>
{
    var info = await tempo.GetTempoOverlayInfoAsync(lat, lon);
    return Results.Ok(info);
});

app.Run();
