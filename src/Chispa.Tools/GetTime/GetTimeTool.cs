using System.Text.Json;
using Chispa.Core.Abstractions;
using System.Globalization;

namespace Chispa.Tools.GetTime;

public sealed class GetTimeTool : ITool
{
    public ToolDefinition Definition { get; } = new(
        Name: "get_time",
        Description: "Returns the current local or UTC date and time.",
        InputSchemaJson:
        """
        {
          "type": "object",
          "properties": {
            "utc": {
              "type": "boolean",
              "description": "Use UTC instead of local time."
            }
          },
          "additionalProperties": false
        }
        """);

    public ValueTask<ToolResult> ExecuteAsync(
        ToolInvocation invocation,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool useUtc =
            invocation.Arguments.TryGetProperty(
                "utc",
                out JsonElement utcArgument) &&
            utcArgument.ValueKind is JsonValueKind.True or JsonValueKind.False &&
            utcArgument.GetBoolean();

        DateTimeOffset currentTime = useUtc
            ? DateTimeOffset.UtcNow
            : DateTimeOffset.Now;

        string formattedTime = currentTime.ToString(
            "yyyy-MM-dd HH:mm:ss zzz",
            CultureInfo.InvariantCulture);

        return ValueTask.FromResult(
            ToolResult.Success(formattedTime));
    }
}