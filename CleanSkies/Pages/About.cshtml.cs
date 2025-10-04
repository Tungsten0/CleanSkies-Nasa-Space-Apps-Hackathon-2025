using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CleanSkies.Pages;

public class AboutModel : PageModel
{
    // Some sample data to show on the page
    public string AppName { get; } = "CleanSkies";
    public string Tagline  { get; } = "Simple Razor Pages app with maps & charts";
    public string Version  { get; } =
        typeof(Program).Assembly.GetName().Version?.ToString() ?? "dev";

    public void OnGet() { }
}
