using Application.Dtos.File;
using Common.Constants;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.BusinessLogic.Queries.File
{
    public class GetFileContentQuery : IRequest<FileBinaryDto>
    {
        public Guid FileId { get; set; }
    }
    public class GetFileContentQueryHandler : IRequestHandler<GetFileContentQuery, FileBinaryDto>
    {
        private readonly ApplicationDbContext _dbContext;
        public GetFileContentQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FileBinaryDto> Handle(GetFileContentQuery request, CancellationToken cancellationToken)
        {
            var tmp = await (from f in _dbContext.Files.AsNoTracking()
                             join c in _dbContext.FileContents.AsNoTracking() on f.Id equals c.FileId
                             where f.Id == request.FileId
                             select new
                             {
                                 f.Name,
                                 f.ContentType,
                                 c.Data
                             })
                            .SingleOrDefaultAsync(cancellationToken);

            if (tmp is null)
                throw new KeyNotFoundException("File not found.");

            var ext = FileTypeRules.NormalizeExtension(Path.GetExtension(tmp.Name));
            if (!FileTypeRules.IsAllowed(ext))
                throw new NotSupportedException("Only .txt, .png, .jpg/.jpeg are allowed.");

            var safeContentType = FileTypeRules.GetContentTypeOrDefault(ext);

            return new FileBinaryDto
            {
                Name = tmp.Name,
                ContentType = safeContentType,
                Data = tmp.Data
            };
        }
    }
}
