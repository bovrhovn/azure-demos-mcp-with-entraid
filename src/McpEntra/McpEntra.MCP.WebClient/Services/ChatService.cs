using Azure.AI.Projects;
using Azure.Identity;
using McpEntra.MCP.WebClient.Options;
using Microsoft.Extensions.Options;

namespace McpEntra.MCP.WebClient.Services;

public class ChatService(ILogger<ChatService> logger, 
    IOptions<AIOptions> options)
{
    public async Task<string> GetResponseAsync(string prompt)
    {
        var url = options.Value.ProjectUrl;
        var deployment = options.Value.DeploymentName;
        logger.LogInformation("Fetching AI response from AI server at {Url} with deployment {Deployment} for prompt: {Prompt}", url, deployment, prompt);
        // var cred = new DefaultAzureCredential();
        // var agent = new AIProjectClient(new Uri(url, UriKind.Absolute), 
        //         cred)
        //     .AsAIAgent(
        //         model: deployment,
        //         name: "McpEntraWebUiAgent",
        //         instructions: "Use the available MCP tools when they can help answer the user's request.");
        // var response = await agent.RunAsync(prompt);
        // logger.LogInformation("Received AI response: {Response}", response.Text);
        // return response.Text;
        return await Task.FromResult("This is a placeholder response from the AI service. The actual implementation is commented out for now: " + prompt);
    }
}