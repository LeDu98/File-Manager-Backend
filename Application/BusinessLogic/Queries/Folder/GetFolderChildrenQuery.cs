using Application.Dtos.File;
using Application.Dtos.Folder;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BusinessLogic.Queries.Folder
{
    public class GetFolderChildrenQuery : IRequest<FolderChildrenDto>
    {
        public Guid? FolderId { get; set; } // null = root
    }

    public class GetFolderChildrenHandler : IRequestHandler<GetFolderChildrenQuery, FolderChildrenDto>
    {
        private readonly ApplicationDbContext _dbContext;
        public GetFolderChildrenHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public async Task<FolderChildrenDto> Handle(GetFolderChildrenQuery req, CancellationToken ct)
        {
            if (req.FolderId.HasValue)
            {
                var exists = await _dbContext.Folders.AsNoTracking()
                    .AnyAsync(f => f.Id == req.FolderId.Value, ct);
                if (!exists) throw new KeyNotFoundException();
            }

            var folders = await _dbContext.Folders.AsNoTracking()
                .Where(_ => _.ParentId == req.FolderId)
                .Select(_ => new FolderDto
                {
                    Id = _.Id,
                    ParentId = _.ParentId,
                    Name = _.Name,
                    CreatedOn = _.CreatedOn,
                    ModifiedOn = _.ModifiedOn
                })
                .ToListAsync();

            var files = await _dbContext.Files.AsNoTracking()
                .Where(_ => _.FolderId == req.FolderId)
                .Select(_ => new FileDto
                {
                    Id = _.Id,
                    Name = _.Name,
                    FolderId = _.FolderId,
                    CreatedOn = _.CreatedOn,
                    ModifiedOn = _.ModifiedOn,
                    ContentType = _.ContentType,
                    SizeInBytes = _.SizeInBytes
                })
                .ToListAsync();

            return new FolderChildrenDto
            {
                FolderId = req.FolderId,
                Folders = folders,
                Files = files
            };
        }
    }
}
