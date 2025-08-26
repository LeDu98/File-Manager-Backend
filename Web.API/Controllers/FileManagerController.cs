using Application.BusinessLogic.Queries.Folder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    }
}
