using RagSystem.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace RagSystem.Presentation.Controllers
{
    public class ApiController : Controller
    {
        protected bool PaginationIsInvalid(int pageNumber, int pageSize)
        {
            return pageNumber <= 0 || pageSize <= 0;
        }

        protected IActionResult HandleFailure(Result result) =>
            result switch
            {
                { IsSuccess: true } => throw new InvalidOperationException(),
                IValidationResult validationResult => BadRequest(
                    CreateProblemDetails(
                        "Erro de validação",
                        StatusCodes.Status400BadRequest,
                        result.Error,
                        validationResult.Errors
                    )
                ),
                _ => BadRequest(
                    CreateProblemDetails(
                        "Requisição inválida",
                        StatusCodes.Status400BadRequest,
                        result.Error
                    )
                ),
            };

        private static ProblemDetails CreateProblemDetails(
            string title,
            int status,
            Error error,
            Error[]? errors = null
        ) =>
            new()
            {
                Title = title,
                Type = error.Code,
                Detail = error.Message,
                Status = status,
                Extensions = { { nameof(errors), errors } },
            };
    }
}
