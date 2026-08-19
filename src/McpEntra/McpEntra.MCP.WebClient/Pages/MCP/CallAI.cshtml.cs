using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McpEntra.MCP.WebClient.Pages.MCP;

[Authorize]
public class CallAIPageModel(ILogger<CallAIPageModel> logger) : PageModel
{
    public void OnGet()
    {
        logger.LogInformation("CallAIPageModel OnGet called at {DateLoaded}", DateTime.UtcNow);
    }
}