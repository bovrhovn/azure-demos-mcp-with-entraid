using Spectre.Console;

#region Environment Variables

AnsiConsole.MarkupLine("[violet]Calling the MCP Entra CLI Client...[/]");
var url = Environment.GetEnvironmentVariable("MCP_URL") ?? "http://localhost:7777";
ArgumentException.ThrowIfNullOrEmpty(url, "MCP_URL environment variable is not set.");
AnsiConsole.MarkupLine($"[blue]Using MCP URL: {url} [/]");

#endregion