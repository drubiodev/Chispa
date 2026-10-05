using System.Text.Json;

namespace Chispa.Core.Abstractions;

public interface ITools
{
    ToolDefinition Definition { get; }

    ValueTask<ToolResult> ExecuteAsync(JsonElement rawInput, CancellationToken cancellationToken);
}