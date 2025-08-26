using Application.BusinessLogic.Commands;
using Application.BusinessLogic.Queries.Folder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Models;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("api/file-manager")]
    public class FileManagerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FileManagerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid?}")]
        public async Task<IActionResult> GetFolderContent(Guid? id, CancellationToken cancellationToken) 
        {
            try
            {
                var vm = await _mediator.Send(new GetFolderChildrenQuery { FolderId = id }, cancellationToken);
                return Ok(vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("breadcrumb/{id:guid?}")]
        public async Task<IActionResult> GetFolderBreadcrumb(Guid? id, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _mediator.Send(new GetFolderBreadcrumbQuery { FolderId = id }, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("batch/delete")]
        public async Task<IActionResult> DeleteItems([FromBody] DeleteItemsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteItemsCommand
                {
                    FolderIds = request.FolderIds,
                    FileIds = request.FileIds
                };

                await _mediator.Send(command, cancellationToken);
                return NoContent(); // 204 No Content
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

