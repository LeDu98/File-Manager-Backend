using Application.Dtos.File;
using Common.Constants;
using Common.Enums;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.BusinessLogic.Commands.File
{
    public class UploadFilesCommand : IRequest<List<Guid>>
    {
        public List<IncomingFile> Files { get; set; }
        public Guid? ParentId { get; set; }
    }
    public class UploadFilesCommandHandler : IRequestHandler<UploadFilesCommand, List<Guid>>
    {
        private readonly ApplicationDbContext _dbContext;
        public UploadFilesCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Guid>> Handle(UploadFilesCommand request, CancellationToken cancellationToken)
        {
            if (request.ParentId.HasValue)
            {
                var exists = await _dbContext.Folders.AsNoTracking()
                    .AnyAsync(f => f.Id == request.ParentId.Value, cancellationToken);
                if (!exists) throw new KeyNotFoundException("Folder not found.");
            }

            var createdIds = new List<Guid>(request.Files.Count);

            await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var f in request.Files)
                {
                    if (f.Data == null || f.Data.Length == 0)
                        throw new ArgumentException("Empty file content.");

                    if (f.Data.LongLength > 50 * 1024 * 1024)
                        throw new ArgumentException("Max 50MB per file in demo.");

                    var originalName = (f.FileName ?? "file").Trim();
                    var stem = Path.GetFileNameWithoutExtension(originalName);
                    if (string.IsNullOrWhiteSpace(stem)) stem = "file";

                    var ext = FileTypeRules.NormalizeExtension(Path.GetExtension(originalName));
                    if (!FileTypeRules.IsAllowed(ext))
                        throw new NotSupportedException("Only .txt, .png, .jpg/.jpeg are allowed.");

                    var contentType = FileTypeRules.GetContentTypeOrDefault(ext);

                    var finalName = $"{stem}{ext}";
                    var i = 1;
                    while (await _dbContext.Files.AsNoTracking()
                               .AnyAsync(x => x.FolderId == request.ParentId && x.Name == finalName, cancellationToken))
                    {
                        finalName = $"{stem} ({i++}){ext}";
                    }

                    var fileEntity = new Domain.Entities.File
                    {
                        Id = Guid.NewGuid(),
                        EntityStatus = eEntityStatus.Active,
                        FolderId = request.ParentId,
                        Name = finalName,
                        ContentType = contentType,
                        SizeInBytes = f.Data.LongLength,
                        CreatedOn = DateTimeOffset.UtcNow,
                        ModifiedOn = DateTimeOffset.UtcNow
                    };

                    var contentEntity = new Domain.Entities.FileContent
                    {
                        FileId = fileEntity.Id,
                        Data = f.Data
                    };

                    _dbContext.Add(fileEntity);
                    _dbContext.Add(contentEntity);
                    createdIds.Add(fileEntity.Id);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }

            return createdIds;
        }
    }

}
