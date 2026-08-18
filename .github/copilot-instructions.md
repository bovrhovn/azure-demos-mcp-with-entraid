# Copilot instructions

## Build and run

Run commands from the repository root. The solution and projects target .NET 10.

```powershell
dotnet restore "src\McpEntra\McpEntra.slnx"
dotnet build "src\McpEntra\McpEntra.slnx" --configuration Release
dotnet format "src\McpEntra\McpEntra.slnx" --verify-no-changes --no-restore
```

Run the stateless HTTP MCP server on the port used by the CLI default:

```powershell
dotnet run --project "src\McpEntra\McpEntra.MCP.Basic\McpEntra.MCP.Basic.csproj" --urls http://localhost:7777
```

Run the companion CLI with an optional server URL override:

```powershell
$env:MCP_URL = "http://localhost:7777"
dotnet run --project "src\McpEntra\McpEntra.MCP.CliClient\McpEntra.MCP.CliClient.csproj"
```

The server project is configured for self-contained, single-file publishing across the runtime identifiers in its project file. Specify one RID when publishing, for example:

```powershell
dotnet publish "src\McpEntra\McpEntra.MCP.Basic\McpEntra.MCP.Basic.csproj" --configuration Release --runtime win-x64
```

## Architecture

- `src\McpEntra\McpEntra.MCP.Basic` is an ASP.NET Core MCP server. `Program.cs` registers `ModelContextProtocol.AspNetCore`, selects stateless HTTP transport, registers tool classes, and maps the MCP endpoint at `/`.
- MCP tools live under the server project's `Tools` directory. The current `RandomNumberTools` class demonstrates the complete discovery path: registration through `.WithTools<T>()`, `[McpServerTool]` on callable methods, and `[Description]` metadata for the tool and its parameters.
- `src\McpEntra\McpEntra.MCP.CliClient` is a separate Spectre.Console executable. It currently resolves `MCP_URL` (defaulting to `http://localhost:7777`) and displays it; it does not yet establish an MCP connection or authenticate.
- `HttpCalls\Basic.http` is the executable protocol example. It sends JSON-RPC `tools/call` requests directly to `/`, requests JSON or server-sent events, and documents the expected MCP protocol header and generated snake_case tool name.
- The repository describes an Entra ID-protected MCP demo, but authentication and authorization are not implemented in the current server or client. Do not assume an Entra middleware, token flow, or configuration contract already exists.

## Azure and Microsoft Entra direction

- The intended architecture is an Azure-hosted MCP server protected by Microsoft Entra ID and OAuth 2.1. Keep the server and CLI changes aligned so that resource identifiers, scopes, redirect URIs, and token validation expectations form one documented contract.
- Implement authentication through ASP.NET Core authentication and authorization middleware around the mapped MCP endpoint, rather than adding token parsing or authorization checks inside individual tool methods.
- Validate access-token signature, issuer, audience, and lifetime. Express tool or endpoint access through named authorization policies backed by delegated scopes or application roles; do not treat successful authentication as sufficient authorization.
- Treat the CLI as a public native client: use authorization code with PKCE for interactive user sign-in, never embed a client secret, and use the Microsoft-supported identity library instead of implementing OAuth exchanges directly.
- Keep tenant IDs, client IDs, audiences, scopes, and Azure resource settings in configuration. Keep credentials and tokens out of source-controlled files, HTTP examples, and console output; use local user secrets or environment variables and managed identity for Azure-hosted workloads where applicable.
- When authentication is added, update `README.md`, the CLI, and `HttpCalls\Basic.http` together with the required Entra app registrations, exposed API scopes or app roles, local configuration keys, and an authenticated end-to-end request.

## Repository conventions

- Keep the HTTP transport stateless unless the feature explicitly requires session state; this is set in `Program.cs` with `options.Stateless = true`.
- Add tool classes to `McpEntra.MCP.Basic\Tools` and explicitly register each class with `.WithTools<T>()`; attributes alone do not add a class to the server.
- Tool methods are public instance methods on internal tool classes. Add `System.ComponentModel.Description` metadata to the method and every exposed parameter because MCP clients use it for tool discovery.
- C# method names are exposed as snake_case MCP tool names (`GetRandomNumber` becomes `get_random_number`). Update `HttpCalls\Basic.http` when changing a tool name or request shape.
- Preserve nullable reference types and implicit global usings, which are enabled in both projects.
- Keep server deployment changes compatible with self-contained, single-file publishing and the declared cross-platform runtime identifiers.
