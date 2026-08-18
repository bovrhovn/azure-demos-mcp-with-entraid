using McpEntra.MCP.Basic.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("McpAccess", policy =>
        policy.RequireClaim("scp", "Mcp.Access"));
});
builder.Services
    .AddMcpServer()
    .WithHttpTransport(options => { options.Stateless = true; })
    .WithTools<RandomNumberTools>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthentication();
app.MapMcp("/mcp").RequireAuthorization("McpAccess");
app.Map("/health", () => Results.Ok($"I am running at {DateTime.Now}")).AllowAnonymous();
app.Run();