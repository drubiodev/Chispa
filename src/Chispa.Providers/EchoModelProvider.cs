using Chispa.Core.Abstractions;

namespace Chispa.Providers;

public sealed class EchoModelProvider : IModelProvider
{
    public Task<ChatMessage> GenerateAsync(
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        ChatMessage? latestUserMessage = messages
            .LastOrDefault(message => message.Role == MessageRole.User);

        string response = latestUserMessage is null
            ? "There are no user messages yet."
            : $"I received: {latestUserMessage.Content}";

        return Task.FromResult(ChatMessage.FromAssistant(response));
    }
}