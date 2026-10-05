namespace Chispa.Core.Abstractions;

public interface IModelProvider
{
    Task<ChatMessage> GenerateAsync(
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken = default);
}