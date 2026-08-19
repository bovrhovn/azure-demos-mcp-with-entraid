using McpEntra.MCP.WebClient.Models;
using McpEntra.MCP.WebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McpEntra.MCP.WebClient.Pages.MCP;

[Authorize]
public class IndexPageModel(ILogger<IndexPageModel> logger, 
    McpService mcpService) : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Handling GET request for MCP Index page at {DateLoaded}.", DateTime.UtcNow);
        var tools = await mcpService.GetMcpToolsAsync();
        logger.LogInformation("Retrieved {ToolCount} tools from MCP service.", tools.Count);
        ToolsInfo = tools;
    }

    [BindProperty(SupportsGet = false)]
    public required List<McpToolInfo> ToolsInfo { get; set; }    
}