using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace McpEntra.MCP.Basic.Tools;

/// <summary>
/// Sample MCP tools for demonstration purposes.
/// These tools can be invoked by MCP clients to perform various operations.
/// </summary>
[AllowAnonymous]
internal class RandomNumberTools(ILogger<RandomNumberTools> logger)
{
    [McpServerTool]
    [Description("Generates a random number between the specified minimum and maximum values.")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")]
        int min = 0,
        [Description("Maximum value (exclusive)")]
        int max = 100)
    {
        logger.LogInformation("Generating random number between {Min} and {Max}", min, max);
        return Random.Shared.Next(min, max);
    }
}