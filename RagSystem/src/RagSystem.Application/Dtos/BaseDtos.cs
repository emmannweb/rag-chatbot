namespace RagSystem.Application.Dtos
{
    public record BaseDto
    {
        public required Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}