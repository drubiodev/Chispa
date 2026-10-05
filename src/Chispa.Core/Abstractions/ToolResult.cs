namespace Chispa.Core.Abstractions;

public sealed record ToolResult
{
    private ToolResult(bool isSuccess, string content)
    {
        IsSuccess = isSuccess;
        Content = content;
    }

    public bool IsSuccess { get; }

    public string Content { get; }

    public static ToolResult Success(string content) =>
        new(true, content);

    public static ToolResult Failure(string message) =>
        new(false, message);
}