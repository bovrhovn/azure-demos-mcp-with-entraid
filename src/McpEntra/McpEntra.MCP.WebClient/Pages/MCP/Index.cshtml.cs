using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McpEntra.MCP.WebClient.Pages.MCP;

public class IndexPageModel(ILogger<IndexPageModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Handling GET request for MCP Index page at {DateLoaded}.", DateTime.UtcNow);
}