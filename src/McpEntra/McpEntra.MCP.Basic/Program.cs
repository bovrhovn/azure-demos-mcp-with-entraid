using McpEntra.MCP.Basic.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using ModelContextProtocol.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme =
            McpAuthenticationDefaults.AuthenticationScheme;
    })
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();
builder.Services
    .AddMcpServer()
    .WithHttpTransport(options => { options.Stateless = true; })
    .WithTools<RandomNumberTools>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapMcp("/mcp").RequireAuthorization();
app.MapGet("/.well-known/oauth-protected-resource", (HttpContext ctx) =>
    {
        var tenantId = builder.Configuration["AzureAd:TenantId"];
        var clientId = builder.Configuration["AzureAd:ClientId"];

        return Results.Json(new
        {
            resource = $"{ctx.Request.Scheme}://{ctx.Request.Host}/mcp",
            authorization_servers = new[]
            {
                $"https://login.microsoftonline.com/{tenantId}/v2.0"
            },
            scopes_supported = new[]
            {
                $"api://{clientId}/MCP.Access"
            }
        });
    })
    .AllowAnonymous();
app.MapGet("/.well-known/oauth-authorization-server", (HttpContext ctx) =>
    {
        var tenantId = builder.Configuration["AzureAd:TenantId"];
        return Results.Json(new
        {
            issuer =
                $"https://login.microsoftonline.com/{tenantId}/v2.0",

            authorization_endpoint =
                $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize",

            token_endpoint =
                $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token",

            jwks_uri =
                $"https://login.microsoftonline.com/{tenantId}/discovery/v2.0/keys"
        });
    })
    .AllowAnonymous();
app.Map("/health", () => Results.Ok($"I am running at {DateTime.Now}")).AllowAnonymous();
app.Run();