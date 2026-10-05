using Chispa.Core.Abstractions;

namespace Chispa.Core.State;

public sealed class Conversation
{
    private readonly List<ChatMessage> _messages = [];

    public IReadOnlyList<ChatMessage> Messages => _messages;

    public int Count => _messages.Count;

    public void Add(ChatMessage message)
    {
        _messages.Add(message);
    }
}