using RagSystem.Application.Dtos.Rag;

namespace RagSystem.Application.UseCases.Rag.Chat
{
    public interface IChatUseCase
    {
        IAsyncEnumerable<string> ExecuteAsync(ChatQuestion question, CancellationToken cancellationToken);
    }
}
