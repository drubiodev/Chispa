using System.Text.Json;

namespace Chispa.Core.Abstractions;

public sealed record ToolInvocation(
    string ToolName,
    JsonElement Arguments);