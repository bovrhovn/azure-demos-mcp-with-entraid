using System.Net.Http.Headers;
using Microsoft.Identity.Client;
using ModelContextProtocol.Client;
using Spectre.Console;

#region Environment Variables

AnsiConsole.MarkupLine("[violet]Calling the MCP Entra CLI Client with OBO...[/]");
var url = Environment.GetEnvironmentVariable("MCP_URL") ?? "http://localhost:7777/mcp";
AnsiConsole.MarkupLine($"[blue]Using MCP URL: {Markup.Escape(url)}[/]");

var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
ArgumentException.ThrowIfNullOrEmpty(clientId, "CLIENT_ID environment variable is required.");
AnsiConsole.MarkupLine($"[blue]Using Client ID: {Markup.Escape(clientId)}[/]");

var mcpClientId = Environment.GetEnvironmentVariable("MCP_CLIENT_ID");
ArgumentException.ThrowIfNullOrEmpty(mcpClientId, "MCP_CLIENT_ID environment variable is required.");
AnsiConsole.MarkupLine($"[blue]Using MCP Client ID: {Markup.Escape(mcpClientId)}[/]");

var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
ArgumentException.ThrowIfNullOrEmpty(tenantId, "TENANT_ID environment variable is required.");
AnsiConsole.MarkupLine($"[blue]Using Tenant ID: {Markup.Escape(tenantId)}[/]");

#endregion

var pca = PublicClientApplicationBuilder
    .Create(clientId)
    .WithTenantId(tenantId)
    .WithRedirectUri("http://localhost")
    .Build();

var result = await pca.AcquireTokenInteractive(
    [
        $"api://{mcpClientId}/MCP.Access"
    ])
    .ExecuteAsync();

static string MaskSecret(string secret, int visibleStart = 4, int visibleEnd = 4)
{
    if (string.IsNullOrEmpty(secret))
        return string.Empty;

    return secret.Length <= visibleStart + visibleEnd ? new string('*', secret.Length) : $"{secret[..visibleStart]}***{secret[^visibleEnd..]}";
}

var userToken = result.AccessToken;
AnsiConsole.MarkupLine($"[blue]Acquired user token: {Markup.Escape(MaskSecret(userToken))}[/]");

var http = new HttpClient();
http.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue(
        "Bearer", userToken);

await using var mcpClient = await McpClient.CreateAsync(
    new HttpClientTransport(
        new HttpClientTransportOptions
        {
            Endpoint = new Uri(url),
            Name = "McpEntraWithObo",
            TransportMode = HttpTransportMode.StreamableHttp
        }, http));
using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

var mcpTools = await mcpClient.ListToolsAsync(
    cancellationToken: cancellationTokenSource.Token);

AnsiConsole.MarkupLine($"[green]Discovered {mcpTools.Count} MCP tool(s):[/]");
foreach (var tool in mcpTools)
{
    AnsiConsole.MarkupLine(
        $"  [yellow]{Markup.Escape(tool.Name)}[/] - {Markup.Escape(tool.Description ?? "No description")}");
}
AnsiConsole.MarkupLine("[green]Done with calling the MCP Entra CLI Client with OBO...[/]");