using System.Text.Json;
using Chispa.Core.Abstractions;
using Chispa.Core.State;
using Chispa.Providers;
using Chispa.Tools.GetTime;

Console.WriteLine("Commands: /history, /time, /utc, /exit");
Console.WriteLine("Press Ctrl+C while the model is thinking to cancel.");
Console.WriteLine();

ITool timeTool = new GetTimeTool();
var conversation = new Conversation();

using var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:11434/"),
    Timeout = TimeSpan.FromMinutes(5)
};

IModelProvider model = new OllamaModelProvider(
    httpClient,
    "gemma4:latest");

CancellationTokenSource? activeRequest = null;

Console.CancelKeyPress += (_, eventArgs) =>
{
    if (activeRequest is null)
    {
        return;
    }

    eventArgs.Cancel = true;
    activeRequest.Cancel();
};

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

    if (input.Equals("/time", StringComparison.OrdinalIgnoreCase))
    {
        await RunTimeToolAsync(useUtc: false);
        continue;
    }

    if (input.Equals("/utc", StringComparison.OrdinalIgnoreCase))
    {
        await RunTimeToolAsync(useUtc: true);
        continue;
    }

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    conversation.Add(ChatMessage.FromUser(input));

    using var requestCancellation = new CancellationTokenSource();
    activeRequest = requestCancellation;

    try
    {
        Console.Write("assistant> thinking...");

        ChatMessage response = await model.GenerateAsync(
            conversation.Messages,
            requestCancellation.Token);

        conversation.Add(response);

        ClearCurrentLine();
        Console.WriteLine($"assistant> {response.Content}");
    }
    catch (OperationCanceledException)
        when (requestCancellation.IsCancellationRequested)
    {
        ClearCurrentLine();
        WriteStatus("request cancelled", ConsoleColor.Yellow);
    }
    catch (TaskCanceledException)
    {
        ClearCurrentLine();
        WriteStatus("Ollama request timed out", ConsoleColor.Red);
    }
    catch (HttpRequestException exception)
    {
        ClearCurrentLine();
        WriteStatus(
            $"could not reach Ollama: {exception.Message}",
            ConsoleColor.Red);
    }
    catch (InvalidDataException exception)
    {
        ClearCurrentLine();
        WriteStatus(
            $"invalid Ollama response: {exception.Message}",
            ConsoleColor.Red);
    }
    finally
    {
        activeRequest = null;
    }
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

static void ClearCurrentLine()
{
    int width = Math.Max(Console.WindowWidth - 1, 1);

    Console.Write('\r');
    Console.Write(new string(' ', width));
    Console.Write('\r');
}

static void WriteStatus(string message, ConsoleColor color)
{
    ConsoleColor previousColor = Console.ForegroundColor;

    Console.ForegroundColor = color;
    Console.WriteLine($"[{message}]");
    Console.ForegroundColor = previousColor;
}

async Task RunTimeToolAsync(bool useUtc)
{
    JsonElement arguments = JsonSerializer.SerializeToElement(
        new { utc = useUtc });

    var invocation = new ToolInvocation(
        timeTool.Definition.Name,
        arguments);

    ToolResult result = await timeTool.ExecuteAsync(invocation);

    ConsoleColor color = result.IsSuccess
        ? ConsoleColor.Green
        : ConsoleColor.Red;

    ConsoleColor previousColor = Console.ForegroundColor;
    Console.ForegroundColor = color;

    Console.WriteLine(
        $"tool[{timeTool.Definition.Name}]> {result.Content}");

    Console.ForegroundColor = previousColor;
}