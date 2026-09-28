using Asp.Versioning;
using RagSystem.Application.UseCases.Rag.UploadDocuments;
using RagSystem.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using RagSystem.Application.UseCases.Rag.Chat;
using RagSystem.Application.Dtos.Rag;

namespace RagSystem.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/rag")]
    public class RagController : ApiController
    {
        public RagController() { }

        [HttpPost("documents")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadDocuments(
            [FromServices] IUploadDocumentsUseCase uploadDocumentsUseCase,
            IFormFile file,
            CancellationToken cancellationToken
        )
        {
            var result = await uploadDocumentsUseCase.ExecuteAsync(file, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result);

            return Ok($"File '{file.FileName}' successfully ingested.");
        }

        [HttpPost("chats")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> Chats(
            [FromServices] IChatUseCase chatUseCase,
            [FromBody] ChatQuestion request,
            CancellationToken cancellationToken
        )
        {
            var stream = chatUseCase.ExecuteAsync(request, cancellationToken);
            return new StreamResult(stream);
        }
    }
}