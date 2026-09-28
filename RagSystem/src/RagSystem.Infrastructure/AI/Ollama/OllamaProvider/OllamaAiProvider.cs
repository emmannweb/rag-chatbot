using RagSystem.Domain.Repositories;
using RagSystem.Infrastructure.AI.Ollama.Configuration;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OllamaSharp.Models;
using System.Runtime.CompilerServices;

namespace RagSystem.Infrastructure.AI.Ollama.OllamaProvider
{
    public class OllamaAiProvider : IAIProvider
    {
        private readonly IOllamaApiClient _ollamaClient;
        private readonly OllamaSettings _settings;

        public OllamaAiProvider(IOllamaApiClient ollamaClient, IOptions<OllamaSettings> settings)
        {
            _ollamaClient = ollamaClient;
            _settings = settings.Value;
        }

        public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken)
        {
            // _ollamaClient.SelectedModel = _settings.EmbeddingModel;
            var request = new EmbedRequest
            {
                Model = _settings.EmbeddingModel,
                Input = [text]
            };
            var response = await _ollamaClient.EmbedAsync(request, cancellationToken);

            if (response?.Embeddings != null && response.Embeddings.Count > 0)
                return response.Embeddings[0].Select(e => (float) e).ToArray();

            throw new InvalidOperationException("Failed to fetch embedding from Ollama.");
        }

        public async Task<string> GenerateCompletionAsync(string prompt, CancellationToken cancellationToken)
        {
            // _ollamaClient.SelectedModel = _settings.ChatModel;
            var request = new GenerateRequest
            {
                Model = _settings.ChatModel,
                Prompt = prompt,
                // Use OllamaSharp's strongly-typed RequestOptions
                Options = new RequestOptions
                {
                    Temperature = 0.0f,
                    NumCtx = _settings.ContextWindow
                }
            };

            string fullResponse = string.Empty;

            await foreach (var streamToken in _ollamaClient.GenerateAsync(request, cancellationToken: cancellationToken))
            {
                if (streamToken != null)
                {
                    fullResponse += streamToken.Response;
                }
            }

            return fullResponse.Trim();
        }

        public async IAsyncEnumerable<string> GenerateStreamingCompletionAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var request = new GenerateRequest
            {
                Model = _settings.ChatModel,
                Prompt = prompt,
                Options = new RequestOptions
                {
                    Temperature = 0.0f,
                    NumCtx = _settings.ContextWindow
                }
            };

            // Stream chunks directly from OllamaSharp using GenerateAsync
            await foreach (var streamToken in _ollamaClient.GenerateAsync(request, cancellationToken: cancellationToken))
            {
                if (streamToken?.Response != null)
                {
                    yield return streamToken.Response;
                }
            }
        }
    }
}

