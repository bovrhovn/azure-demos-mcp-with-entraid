using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace McpEntra.MCP.Basic.Tools;

[Authorize]
internal class ProtectedServiceTools(ILogger<ProtectedServiceTools> logger)
{
    [McpServerTool]
    [Description("Generates random person data.")]
    public List<RandomPerson> GenerateRandomPersons(
        [Description("Maximum value (exclusive)")]
        int max = 100)
    {
        logger.LogInformation("Generating random persons with {Max} counter", max);
        var list = new List<RandomPerson>();
        for (var counter = 0; counter < max; counter++)
        {
            list.Add(new RandomPerson(
                FirstName: $"FirstName {counter}",
                LastName: $"LastName {counter}",
                Age: Random.Shared.Next(1, 70)
            ));
        }
        return list;
    }
}

public record RandomPerson(string FirstName, string LastName, int Age);