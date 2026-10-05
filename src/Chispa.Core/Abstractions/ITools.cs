using System.Text.Json;

namespace Chispa.Core.Abstractions;

public interface ITool
{
    ToolDefinition Definition { get; }

    ValueTask<ToolResult> ExecuteAsync(ToolInvocation invocation, CancellationToken cancellationToken = default);
}