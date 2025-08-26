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
    public class GetFolderBreadcrumbQuery : IRequest<List<FolderBreadcrumbDto>>
    {
        public Guid? FolderId { get; set; } //null = root
    }
    public class GetFolderBreadcrumbHandler : IRequestHandler<GetFolderBreadcrumbQuery, List<FolderBreadcrumbDto>>
    {
        private readonly ApplicationDbContext _dbContext;
        public GetFolderBreadcrumbHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<FolderBreadcrumbDto>> Handle(GetFolderBreadcrumbQuery request, CancellationToken cancellationToken)
        {
            if (!request.FolderId.HasValue) return new();

            var rows = await _dbContext.Database.SqlQuery<FolderBreadcrumbDto>($@"
                WITH RECURSIVE chain AS (
                  SELECT f.""Id"", f.""Name"", f.""ParentId"", 0 AS lvl
                  FROM ""Folders"" f
                  WHERE f.""Id"" = {request.FolderId.Value}
                  UNION ALL
                  SELECT p.""Id"", p.""Name"", p.""ParentId"", c.lvl + 1
                  FROM ""Folders"" p
                  JOIN chain c ON p.""Id"" = c.""ParentId""
                  WHERE c.lvl < 128
                )
                SELECT
                  ""Id"",
                  ""Name"",
                  (MAX(lvl) OVER ()) - lvl AS ""Level""
                FROM chain
                ORDER BY lvl DESC;
                ").ToListAsync(cancellationToken);

            if (rows.Count == 0) throw new KeyNotFoundException($"Folder not found: {request.FolderId}");

            return rows;
        }
    }
}