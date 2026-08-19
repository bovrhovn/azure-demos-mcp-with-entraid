#region Environment Variables

using Azure.AI.Projects;
using Azure.Identity;
using McpEntra.MCP.CliClient;
using Microsoft.Agents.AI;
using ModelContextProtocol.Client;
using Spectre.Console;

AnsiConsole.MarkupLine("[violet]Calling the MCP Entra CLI Client...[/]");
var url = Environment.GetEnvironmentVariable("MCP_URL") ?? "http://localhost:7777/mcp";
AnsiConsole.MarkupLine($"[blue]Using MCP URL: {Markup.Escape(url)}[/]");

var deploymentName = Environment.GetEnvironmentVariable("MCP_DEPLOYMENT_NAME") ?? "gpt-5.4";
AnsiConsole.MarkupLine($"[blue]Using Azure Foundry Deployment Name: {Markup.Escape(deploymentName)}[/]");

var projectEndpoint = Environment.GetEnvironmentVariable("AZURE_AI_PROJECT_ENDPOINT");
ArgumentException.ThrowIfNullOrEmpty(projectEndpoint, "AZURE_AI_PROJECT_ENDPOINT environment variable is required.");
AnsiConsole.MarkupLine($"[blue]Using Azure Foundry Project Endpoint: {Markup.Escape(projectEndpoint)}[/]");

#endregion

using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

var credential = new DefaultAzureCredential();
var httpClient = new HttpClient(
    new McpAuthHandler(credential) { InnerHandler = new HttpClientHandler() });

await using var mcpClient = await McpClient.CreateAsync(
    new HttpClientTransport(
        new HttpClientTransportOptions
        {
            Endpoint = new Uri(url),
            Name = "McpEntra",
            TransportMode = HttpTransportMode.StreamableHttp
        }, httpClient));

var mcpTools = await mcpClient.ListToolsAsync(
    cancellationToken: cancellationTokenSource.Token);

AnsiConsole.MarkupLine($"[green]Discovered {mcpTools.Count} MCP tool(s):[/]");
foreach (var tool in mcpTools)
{
    AnsiConsole.MarkupLine(
        $"  [yellow]{Markup.Escape(tool.Name)}[/] - {Markup.Escape(tool.Description ?? "No description")}");
}

AIAgent agent = new AIProjectClient(new Uri(projectEndpoint, UriKind.Absolute), 
        new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ExcludeVisualStudioCredential = true,
            ExcludeAzureCliCredential = false
        }))
    .AsAIAgent(
        model: deploymentName,
        name: "McpEntraCliAgent",
        instructions: "Use the available MCP tools when they can help answer the user's request.",
        tools: [.. mcpTools]);

AnsiConsole.MarkupLine("[grey]Enter a request, or type 'exit' to quit.[/]");
while (!cancellationTokenSource.IsCancellationRequested)
{
    var prompt = AnsiConsole.Ask<string>("[cyan]>[/] ").Trim();
    if (prompt.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    if (prompt.Length == 0)
        continue;

    var response = await agent.RunAsync(prompt, cancellationToken: cancellationTokenSource.Token);
    AnsiConsole.WriteLine(response.ToString());
}