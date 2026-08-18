# azure-demos-mcp-with-entraid
Demo example of having Azure Entra Id to protect MCP server written in C#.

## CLI client

The CLI discovers tools from the MCP server and makes them available to a Microsoft Agent Framework agent backed by an Azure Foundry deployment. Authenticate with `az login`, then configure and run it:

```powershell
$env:MCP_URL = "http://localhost:7777/mcp"
$env:MCP_DEPLOYMENT_NAME = "<deployment-name>"
$env:AZURE_AI_PROJECT_ENDPOINT = "https://<resource>.services.ai.azure.com/api/projects/<project>"
dotnet run --project "src\McpEntra\McpEntra.MCP.CliClient\McpEntra.MCP.CliClient.csproj"
```

Enter natural-language requests at the prompt. The agent selects and calls the discovered MCP tools; enter `exit` to stop.
