using Pgvector;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using RagSystem.Domain.Entities;
using RagSystem.Domain.Repositories;

namespace RagSystem.Infrastructure.Persistence.Repositories
{
    public class RagRepository : IRagRepository
    {
        private readonly ApplicationDbContext _context;

        public RagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddChunkAsync(DocumentChunk chunk, CancellationToken cancellationToken)
        {
            await _context.DocumentChunks.AddAsync(chunk, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> SearchSimilarAsync(float[] queryEmbedding, int limit, CancellationToken cancellationToken)
        {
            var queryVector = new Vector(queryEmbedding);
            return await _context.DocumentChunks
            // Use EF.Property to treat the converted property as a Pgvector.Vector in the LINQ translation tree
            .OrderBy(c => EF.Property<Vector>(c, nameof(DocumentChunk.Embedding)).CosineDistance(queryVector))
            .Take(limit)
            .Select(c => c.Content)
            .ToListAsync(cancellationToken);
        }
    }
}
