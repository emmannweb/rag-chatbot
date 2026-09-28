namespace RagSystem.Domain.Repositories
{
    public interface IAIProvider
    {
        Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken);
        Task<string> GenerateCompletionAsync(string prompt, CancellationToken cancellationToken);
        IAsyncEnumerable<string> GenerateStreamingCompletionAsync(string prompt, CancellationToken cancellationToken);
    }
}
