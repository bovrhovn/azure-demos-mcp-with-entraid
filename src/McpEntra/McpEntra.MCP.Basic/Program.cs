using McpEntra.MCP.Basic.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
app.Map("/health", () => Results.Ok($"I am running at {DateTime.Now}")).AllowAnonymous();
app.Run();