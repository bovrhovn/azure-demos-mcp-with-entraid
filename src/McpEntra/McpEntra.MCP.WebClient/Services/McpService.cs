using System.Net.Http.Headers;
using McpEntra.MCP.WebClient.Models;
using McpEntra.MCP.WebClient.Options;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using ModelContextProtocol.Client;

namespace McpEntra.MCP.WebClient.Services;

public class McpService(
    ILogger<McpService> logger,
    ITokenAcquisition tokenAcquisition,
    IOptions<McpOptions> options,
    IHttpClientFactory httpClientFactory)
{
    public const string HttpClientName = "WebUIMcpServer";

    public async Task<string> GetAccessTokenAsync()
    {
        return await tokenAcquisition
            .GetAccessTokenForUserAsync(
            [
                options.Value.McpApiUrl
            ]);
    }
    
    public async Task<List<McpToolInfo>> GetMcpToolsAsync()
    {
        var url = options.Value.BaseUrl;
        logger.LogInformation("Fetching MCP tools from mcp server at {Url}", url);
        var token = await GetAccessTokenAsync();
        var httpClient = httpClientFactory.CreateClient(HttpClientName);
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        await using var mcpClient = await McpClient.CreateAsync(
            new HttpClientTransport(
                new HttpClientTransportOptions
                {
                    Endpoint = new Uri(url),
                    Name = "McpEntraWebUI",
                    TransportMode = HttpTransportMode.StreamableHttp
                }, httpClient));

        var mcpTools = await mcpClient.ListToolsAsync();
        logger.LogInformation("Discovered {Count} MCP tool(s)", mcpTools.Count);
        var list = new List<McpToolInfo>();
        foreach (var tool in mcpTools)
        {
            list.Add(new McpToolInfo
            {
                Name = tool.Name,
                Description = tool.Description ?? "No description"
            });
        }

        return list;
    }
}