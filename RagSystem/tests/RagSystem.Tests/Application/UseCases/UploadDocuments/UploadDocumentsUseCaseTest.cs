using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using RagSystem.Application.UseCases.Rag.UploadDocuments;
using RagSystem.Domain.Entities;
using RagSystem.Domain.Errors;
using RagSystem.Domain.Repositories;

namespace RagSystem.Tests.Application.UseCases.UploadDocuments
{
    public class UploadDocumentsUseCaseTest
    {
        private readonly Mock<IAIProvider> _aiProvider;
        private readonly Mock<IRagRepository> _ragRepositoryMock;
        private readonly IUploadDocumentsUseCase _uploadDocumentsUseCase;

        public UploadDocumentsUseCaseTest()
        {
            _aiProvider = new Mock<IAIProvider>();
            _ragRepositoryMock = new Mock<IRagRepository>();
            _uploadDocumentsUseCase = new UploadDocumentsUseCase(_aiProvider.Object, _ragRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenFileIsNull()
        {
            // Arrange
            IFormFile? file = null;

            // Act
            var result = await _uploadDocumentsUseCase.ExecuteAsync(file!, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(DomainErrors.DocumentChunk.InvalidFile, result.Error);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnSuccess_WhenFileIsValid()
        {
            // Arrange
            const string fileName = "sample.txt";
            const string content = "First chunk. Second chunk.";
            var file = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            file.Setup(f => f.FileName).Returns(fileName);
            file.Setup(f => f.Length).Returns(stream.Length);
            file.Setup(f => f.OpenReadStream()).Returns(stream);

            _aiProvider
                .Setup(p => p.GetEmbeddingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { 0.1f, 0.2f, 0.3f });

            _ragRepositoryMock
                .Setup(r => r.AddChunkAsync(It.IsAny<DocumentChunk>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _ragRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _uploadDocumentsUseCase.ExecuteAsync(file.Object, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _aiProvider.Verify(p => p.GetEmbeddingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _ragRepositoryMock.Verify(r => r.AddChunkAsync(It.IsAny<DocumentChunk>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _ragRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}