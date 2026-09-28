
using Microsoft.AspNetCore.Http;
using RagSystem.Shared.Results;

namespace RagSystem.Application.UseCases.Rag.UploadDocuments
{
    public interface IUploadDocumentsUseCase
    {
        Task<Result> ExecuteAsync(IFormFile file, CancellationToken cancellationToken);
    }
}
