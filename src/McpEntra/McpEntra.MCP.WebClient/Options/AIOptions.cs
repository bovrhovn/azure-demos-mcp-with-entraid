namespace McpEntra.MCP.WebClient.Options;

public sealed class AIOptions
{
    public const string SectionName = "AI";
    public required string DeploymentName { get; set; }
    public required string ProjectUrl { get; set; }
}