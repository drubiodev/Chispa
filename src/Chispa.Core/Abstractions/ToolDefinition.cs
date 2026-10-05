using System.Text.Json.Nodes;

namespace Chispa.Core.Abstractions;

public sealed record ToolDefinition(
    string Name,
    string Description,
    string InputSchemaJson);