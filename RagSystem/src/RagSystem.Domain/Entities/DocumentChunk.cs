using RagSystem.Domain.Errors;
using RagSystem.Shared.Results;

namespace RagSystem.Domain.Entities
{
    public class DocumentChunk
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }

        // Pure C# type - completely decoupled from PostgreSQL or Pgvector
        public float[] Embedding { get; set; } = Array.Empty<float>();

        private DocumentChunk(
            string fileName,
            string content,
            float[] embedding
        )
        {
            Id = Guid.NewGuid();
            FileName = fileName;
            Content = content;
            Embedding = embedding;
        }

        public static Result<DocumentChunk> Create(
            string fileName,
            string content,
            float[] embedding
        )
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Result.Failure<DocumentChunk>(DomainErrors.DocumentChunk.FileNameIsRequired);

            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<DocumentChunk>(DomainErrors.DocumentChunk.ContentIsRequired);

            if (embedding == null || embedding.Length == 0)
                return Result.Failure<DocumentChunk>(DomainErrors.DocumentChunk.EmbeddingIsRequired);

            return Result.Success(new DocumentChunk(fileName, content, embedding));
        }
    }
}
