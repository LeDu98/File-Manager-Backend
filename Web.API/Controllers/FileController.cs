using Application.BusinessLogic.Commands.File;
using Application.BusinessLogic.Commands.Folder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("rename")]
        public async Task<IActionResult> Rename(RenameItemRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(new RenameFileCommand { Id = request.Id, Name = request.Name }, cancellationToken);
                return Ok(response);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
