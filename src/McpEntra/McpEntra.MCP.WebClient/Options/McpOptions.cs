namespace McpEntra.MCP.WebClient.Options;

public sealed class McpOptions
{
    public const string  SectionName = "Mcp";
    public required string BaseUrl { get; set; }
    public required string McpApiUrl { get; set; }
}