using Microsoft.AspNetCore.Http;
using RagSystem.Domain.Entities;
using RagSystem.Domain.Errors;
using RagSystem.Domain.Repositories;
using RagSystem.Shared.Results;

namespace RagSystem.Application.UseCases.Rag.UploadDocuments
{
    public class UploadDocumentsUseCase : IUploadDocumentsUseCase
    {
        private readonly IAIProvider _aiProvider;
        private readonly IRagRepository _ragRepository;

        public UploadDocumentsUseCase(IAIProvider aiProvider, IRagRepository ragRepository)
        {
            _aiProvider = aiProvider;
            _ragRepository = ragRepository;
        }

        public async Task<Result> ExecuteAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                return Result.Failure(DomainErrors.DocumentChunk.InvalidFile);

            using var reader = new StreamReader(file.OpenReadStream());
            var text = await reader.ReadToEndAsync(cancellationToken);

            var chunks = text.Split(new[] { "\n\n", ". " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var chunkText in chunks)
            {
                if (string.IsNullOrWhiteSpace(chunkText)) continue;

                string cleanChunk = chunkText.Trim();
                float[] embeddingArray = await _aiProvider.GetEmbeddingAsync(cleanChunk, cancellationToken);

                var chunkResult = DocumentChunk.Create(file.FileName, cleanChunk, embeddingArray);

                if (chunkResult.IsFailure)
                    return Result.Failure(chunkResult.Error);

                await _ragRepository.AddChunkAsync(chunkResult.Value, cancellationToken);
            }

            await _ragRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

