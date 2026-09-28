using System.Runtime.CompilerServices;
using RagSystem.Application.Dtos.Rag;
using RagSystem.Domain.Repositories;

namespace RagSystem.Application.UseCases.Rag.Chat
{
    public class ChatUseCase : IChatUseCase
    {
        private readonly IAIProvider _aiProvider;
        private readonly IRagRepository _ragRepository;
        private const int MaxContextChunks = 3; // Limit the number of chunks to include in the context

        public ChatUseCase(IAIProvider aiProvider, IRagRepository ragRepository)
        {
            _aiProvider = aiProvider;
            _ragRepository = ragRepository;
        }

        public async IAsyncEnumerable<string> ExecuteAsync(ChatQuestion request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            float[] queryEmbedding = await _aiProvider.GetEmbeddingAsync(request.Question, cancellationToken);
            var similarChunks = await _ragRepository.SearchSimilarAsync(queryEmbedding, MaxContextChunks, cancellationToken);

            if (similarChunks == null || !similarChunks.Any())
            {
                yield return "I cannot find the answer in the uploaded documents.";
                yield break;
            }

            string context = string.Join("\n---\n", similarChunks);

            // Strict prompt engineering to restrict model outputs to ingested document data only
            string prompt = $"""
                <|start_header_id|>system<|end_header_id|>
                You are a strict, factual assistant for Inova Tech Enterprise. 
                Your ONLY knowledge base is the provided Context. 
                - Answer the question completely based on the Context.
                - Do NOT use outside knowledge, assumptions, or general truths.
                - If the user is asking a general question (like greetings or identity)
                 that doesn't require documents, answer politely.
                - If it's a document-specific question and the answer cannot be found
                 explicitly within the Context, you MUST output exact text: "I cannot find the answer in the uploaded documents."
                <|eot_id|><|start_header_id|>user<|end_header_id|>
                Context:
                {context}

                Question: {request.Question}
                <|eot_id|><|start_header_id|>assistant<|end_header_id|>
                """;
            // Delegate streaming chunk iteration from your AI provider layer
            await foreach (var chunk in _aiProvider.GenerateStreamingCompletionAsync(prompt, cancellationToken))
            {
                yield return chunk;
            }
        }
    }
}

