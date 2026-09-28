
using RagSystem.Domain.Entities;

namespace RagSystem.Domain.Repositories
{
    public interface IRagRepository
    {
        Task AddChunkAsync(DocumentChunk chunk, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<string>> SearchSimilarAsync(float[] queryEmbedding, int limit, CancellationToken cancellationToken);
    }
}
