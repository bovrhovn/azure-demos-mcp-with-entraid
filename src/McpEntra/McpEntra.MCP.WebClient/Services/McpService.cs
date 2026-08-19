using McpEntra.MCP.WebClient.Models;
using McpEntra.MCP.WebClient.Options;
using Microsoft.Extensions.Options;

namespace McpEntra.MCP.WebClient.Services;

public class McpService(ILogger<McpService> logger, IOptions<McpOptions> options)
{
    public Task<List<McpToolInfo>> GetMcpToolsAsync()
    {
        var url = options.Value.BaseUrl;
        logger.LogInformation("Fetching MCP tools from mcp server at {Url}", url);
        return Task.FromResult<List<McpToolInfo>>( 
        [
            new McpToolInfo
            {
                Name = "MCP Tool 1",
                Description = "Description for MCP Tool 1"
            },

            new McpToolInfo
            {
                Name = "MCP Tool 2",
                Description = "Description for MCP Tool 2"
            }
        ]);
    }
}