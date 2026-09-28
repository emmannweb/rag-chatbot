using Moq;
using RagSystem.Application.Dtos.Rag;
using RagSystem.Application.UseCases.Rag.Chat;
using RagSystem.Domain.Repositories;

namespace RagSystem.Tests.Application.UseCases.Chat
{
    public class ChatUseCaseTest
    {
        private readonly Mock<IAIProvider> _aiProvider;
        private readonly Mock<IRagRepository> _ragRepositoryMock;
        private readonly IChatUseCase _chatUseCase;

        public ChatUseCaseTest()
        {
            _aiProvider = new Mock<IAIProvider>();
            _ragRepositoryMock = new Mock<IRagRepository>();
            _chatUseCase = new ChatUseCase(_aiProvider.Object, _ragRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldYieldAiResponse_WhenQueryIsProvided()
        {
            // Arrange
            var query = new ChatQuestion { Question = "What is the capital of France?" };

            _aiProvider
                .Setup(provider => provider.GetEmbeddingAsync(query.Question, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { 0.12f, 0.45f, 0.88f });

            _ragRepositoryMock
                .Setup(repo => repo.SearchSimilarAsync(It.IsAny<float[]>(), 3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "The capital of France is Paris." });

            _aiProvider
                .Setup(provider => provider.GenerateStreamingCompletionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(GetStreamingResponse("The capital of France is Paris."));

            // Act
            var response = new List<string>();
            await foreach (var chunk in _chatUseCase.ExecuteAsync(query, CancellationToken.None))
            {
                response.Add(chunk);
            }

            // Assert
            Assert.NotEmpty(response);
            Assert.Contains(response, chunk => chunk.Contains("Paris"));
        }

        private static async IAsyncEnumerable<string> GetStreamingResponse(string message)
        {
            await Task.Yield();
            yield return message;
        }
    }
}