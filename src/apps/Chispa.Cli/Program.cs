using Chispa.Core.Abstractions;
using Chispa.Core.State;

Console.WriteLine("Harness conversation demo");
Console.WriteLine("Commands: /history, /exit");
Console.WriteLine();

var conversation = new Conversation();

while (true)
{
    Console.Write("you> ");
    string? input = Console.ReadLine();

    if (input is null ||
        input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (input.Equals("/history", StringComparison.OrdinalIgnoreCase))
    {
        PrintHistory(conversation);
        continue;
    }

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    conversation.Add(ChatMessage.FromUser(input));

    string reply = $"I received: {input}";
    conversation.Add(ChatMessage.FromAssistant(reply));

    Console.WriteLine($"assistant> {reply}");
}

static void PrintHistory(Conversation conversation)
{
    Console.WriteLine();
    Console.WriteLine("--- conversation history ---");

    if (conversation.Count == 0)
    {
        Console.WriteLine("(empty)");
    }

    foreach (ChatMessage message in conversation.Messages)
    {
        string speaker = message.Role switch
        {
            MessageRole.User => "you",
            MessageRole.Assistant => "assistant",
            MessageRole.System => "system",
            MessageRole.Tool => "tool",
            _ => "unknown"
        };

        Console.WriteLine($"{speaker}> {message.Content}");
    }

    Console.WriteLine("----------------------------");
    Console.WriteLine();
}
