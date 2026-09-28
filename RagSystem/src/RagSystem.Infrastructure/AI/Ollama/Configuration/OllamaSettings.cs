namespace RagSystem.Infrastructure.AI.Ollama.Configuration
{
    public class OllamaSettings
    {
        public const string SectionName = "Ollama";
        public string BaseUrl { get; set; } = "http://localhost:11434";
        public string ChatModel { get; set; } = "llama3.2:latest";
        public string EmbeddingModel { get; set; } = "nomic-embed-text";
        public int ContextWindow { get; set; } = 2048; // Default to 2048 to save CPU
    }
}

