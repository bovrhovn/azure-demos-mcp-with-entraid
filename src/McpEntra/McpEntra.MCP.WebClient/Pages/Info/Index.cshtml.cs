using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McpEntra.MCP.WebClient.Pages.Info;

[Authorize]
public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Handling GET request for Index page at {DateLoaded}.", DateTime.UtcNow);
}