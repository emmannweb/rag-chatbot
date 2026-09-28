namespace RagSystem.Shared.Results
{
    public interface IValidationResult
    {
        public static readonly Error ValidationError = new(
            "ValidationError",
            "Ocorreu um problema de validação.");

        Error[] Errors { get; }
    }
}
