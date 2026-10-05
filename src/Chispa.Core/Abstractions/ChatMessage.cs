namespace Chispa.Core.Abstractions;

public enum MessageRole
{
    System,
    User,
    Assistant,
    Tool
}

public sealed record ChatMessage
{
    public required MessageRole Role { get; init; }

    public required string Content { get; init; }

    public static ChatMessage FromSystem(string content) =>
        new()
        {
            Role = MessageRole.System,
            Content = content
        };

    public static ChatMessage FromUser(string content) =>
        new()
        {
            Role = MessageRole.User,
            Content = content
        };

    public static ChatMessage FromAssistant(string content) =>
        new()
        {
            Role = MessageRole.Assistant,
            Content = content
        };

    public static ChatMessage FromTool(string content) =>
        new()
        {
            Role = MessageRole.Tool,
            Content = content
        };
}