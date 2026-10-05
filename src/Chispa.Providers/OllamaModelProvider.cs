using System.Net.Http.Json;
using Chispa.Core.Abstractions;

namespace Chispa.Providers;

public sealed class OllamaModelProvider(HttpClient httpClient, string model) : IModelProvider
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _model = model;

    public async Task<ChatMessage> GenerateAsync(
            IReadOnlyList<ChatMessage> messages,
            CancellationToken cancellationToken = default)
    {
        var ollamaMessages = messages
            .Select(message => new OllamaMessage(
                ConvertRole(message.Role),
                message.Content))
            .ToArray();

        var request = new OllamaChatRequest(
            Model: _model,
            Messages: ollamaMessages,
            Stream: false);

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "api/chat",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                $"Ollama returned {(int)response.StatusCode}: {error}");
        }

        OllamaChatResponse? result =
            await response.Content.ReadFromJsonAsync<OllamaChatResponse>(
                cancellationToken: cancellationToken);

        if (result?.Message is null)
        {
            throw new InvalidDataException(
                "Ollama returned a response without a message.");
        }

        return ChatMessage.FromAssistant(result.Message.Content);
    }

    private static string ConvertRole(MessageRole role)
    {
        return role switch
        {
            MessageRole.System => "system",
            MessageRole.User => "user",
            MessageRole.Assistant => "assistant",
            MessageRole.Tool => "tool",
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };
    }

    private sealed record OllamaChatRequest(
        string Model,
        IReadOnlyList<OllamaMessage> Messages,
        bool Stream);

    private sealed record OllamaChatResponse(
        OllamaMessage Message);

    private sealed record OllamaMessage(
        string Role,
        string Content);
}