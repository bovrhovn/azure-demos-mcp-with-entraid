using System.ComponentModel.DataAnnotations;
using McpEntra.MCP.WebClient.Models;
using McpEntra.MCP.WebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace McpEntra.MCP.WebClient.Pages.MCP;

[Authorize]
public class CallAIPageModel(
    ILogger<CallAIPageModel> logger,
    ChatService chatService,
    McpService mcpService) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Write a prompt before sending it.")]
    [StringLength(4_000, ErrorMessage = "Keep the prompt to 4,000 characters or fewer.")]
    public string Prompt { get; set; } = string.Empty;

    public string? AiResponse { get; private set; }

    public List<McpToolInfo> ToolsInfo { get; private set; } = [];

    public bool HasResponse => !string.IsNullOrWhiteSpace(AiResponse);

    public async Task OnGetAsync()
    {
        logger.LogInformation("CallAIPageModel OnGet called at {DateLoaded}", DateTime.UtcNow);
        await LoadToolsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadToolsAsync();
            return Page();
        }

        logger.LogInformation("Sending an AI prompt at {DateLoaded}", DateTime.UtcNow);
        AiResponse = await chatService.GetResponseAsync(Prompt);
        await LoadToolsAsync();
        return Page();
    }

    private async Task LoadToolsAsync()
    {
        ToolsInfo = await mcpService.GetMcpToolsAsync();
    }
}