using System.Text.Json;
using System.Text.Json.Nodes;
using Chispa.Core.Abstractions;

public sealed class GetTimeTool : ITools
{
    public ToolDefinition Definition => new ToolDefinition(
        Name: "GetTime",
        Description: "Gets the current time.",
        InputSchema: new JsonObject()
    );

    public ValueTask<ToolResult> ExecuteAsync(JsonElement rawInput, CancellationToken cancellationToken)
    {
        var currentTime = DateTime.UtcNow;
        var result = new JsonObject
        {
            ["currentTime"] = currentTime.ToString("o")
        };

        return ValueTask.FromResult(new ToolResult(
            Name: "GetTime",
            Output: result
        ));
    }
}