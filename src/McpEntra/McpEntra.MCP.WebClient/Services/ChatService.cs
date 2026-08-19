using McpEntra.MCP.WebClient.Options;
using Microsoft.Extensions.Options;

namespace McpEntra.MCP.WebClient.Services;

public class ChatService(ILogger<ChatService> logger, 
    IOptions<AIOptions> options)
{
    public Task<string> GetResponseAsync(string prompt)
    {
        var url = options.Value.ProjectUrl;
        var deployment = options.Value.DeploymentName;
        logger.LogInformation("Fetching AI response from AI server at {Url} with deployment {Deployment} for prompt: {Prompt}", url, deployment, prompt);
        // Simulate an AI response for demonstration purposes
        return Task.FromResult($"Simulated AI response for prompt: '{prompt}'");
    }
}