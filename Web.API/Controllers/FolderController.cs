using Application.BusinessLogic.Commands;
using Application.BusinessLogic.Commands.Folder;
using Application.BusinessLogic.Queries.Folder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("api/folder")]
    public class FolderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FolderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateFolderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(new CreateFolderCommand { Name = request.Name, ParentId = request.ParentId }, cancellationToken);
                return Ok(response);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
