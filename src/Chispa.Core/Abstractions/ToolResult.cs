using System.Text.Json.Nodes;

namespace Chispa.Core.Abstractions;

public sealed record ToolResult(
    string Name,
    JsonObject Output);