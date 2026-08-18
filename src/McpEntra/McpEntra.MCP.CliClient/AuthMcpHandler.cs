using Spectre.Console;

namespace McpEntra.MCP.CliClient;

using Azure.Core;
using System.Net.Http.Headers;

public sealed class McpAuthHandler(TokenCredential credential) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var mcpApiEndpoint = Environment.GetEnvironmentVariable("MCP_API_ENDPOINT") ?? "https://mcp.azure.com/.default";
        AnsiConsole.MarkupLine($"[blue]Using MCP API Endpoint: {Markup.Escape(mcpApiEndpoint)}[/]");
        var token = await credential.GetTokenAsync(
            new TokenRequestContext([
                mcpApiEndpoint
            ]),
            cancellationToken);
        var currentAuthToken = token.Token;
        AnsiConsole.MarkupLine($"[blue]Using MCP API Token: {Markup.Escape(currentAuthToken)}[/]");
        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                currentAuthToken);

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}