using Application.BusinessLogic.Commands.File;
using Application.BusinessLogic.Commands.Folder;
using Application.BusinessLogic.Queries.File;
using Application.Dtos.File;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request, CancellationToken cancellationToken)
        {
            if (request.Files is null || request.Files.Count == 0)
                return BadRequest("Empty file.");

            var files = new List<IncomingFile>();
            foreach (var f in request.Files)
            {
                if (f.Length == 0) continue;

                using var ms = new MemoryStream();
                await f.CopyToAsync(ms, cancellationToken);

                files.Add(new IncomingFile
                {
                    FileName = f.FileName,
                    ContentType = string.IsNullOrWhiteSpace(f.ContentType) ? "application/octet-stream" : f.ContentType,
                    Data = ms.ToArray()
                });
            }

            if (files.Count == 0) return BadRequest("All files were empty.");

            var ids = await _mediator.Send(new UploadFilesCommand { ParentId = request.ParentId, Files = files}, cancellationToken);
            return Ok(new { ids });
        }

        [HttpGet("{id:guid}/content")]
        public async Task<IActionResult> GetContent(Guid id, CancellationToken ct)
        {
            var dto = await _mediator.Send(new GetFileContentQuery { FileId = id }, ct);
            return File(dto.Data, dto.ContentType);
        }
    }
}
