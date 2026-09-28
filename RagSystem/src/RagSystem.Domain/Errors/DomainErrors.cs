using RagSystem.Shared.Results;

namespace RagSystem.Domain.Errors;

public static class DomainErrors
{
    public static class Commons
    {
        public static readonly Error InvalidPagination = new(
            "Common.InvalidPagination",
            "Formato de parâmetros de paginação incorretos."
        );
        public static readonly Func<Guid, Error> IdIsInvalid = id => new Error(
            "Common.IdIsInvalid",
            $"O identificador {id} é inválido."
        );
    }

    public static class DocumentChunk
    {
        public static readonly Error FileNameIsRequired = new(
            "DocumentChunk.FileNameIsRequired",
            "File name is required."
        );
        public static readonly Error ContentIsRequired = new(
            "DocumentChunk.ContentIsRequired",
            "Content is required."
        );
        public static readonly Error EmbeddingIsRequired = new(
            "DocumentChunk.EmbeddingIsRequired",
            "Embedding is required."
        );
        public static readonly Error NotFound = new(
            "DocumentChunk.NotFound",
            "Document chunk not found."
        );
        public static readonly Error RemoveFailed = new(
            "DocumentChunk.RemoveFailed",
            "Failed to remove document chunk."
        );
        public static readonly Error InvalidFile = new(
            "DocumentChunk.InvalidFile",
            "Invalid file provided."
        );
    }

}
