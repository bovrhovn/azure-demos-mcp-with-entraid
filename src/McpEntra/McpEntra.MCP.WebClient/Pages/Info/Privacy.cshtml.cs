using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McpEntra.MCP.WebClient.Pages.Info;

[AllowAnonymous]
public class PrivacyModel(ILogger<PrivacyModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Handling GET request for Privacy page at {DateLoaded}.", DateTime.UtcNow);
}