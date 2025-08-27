using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BusinessLogic.Commands.Folder
{
    public class CreateFolderCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
    }
    public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, Guid>
    {
        private readonly ApplicationDbContext _dbContext;
        public CreateFolderCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Guid> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            var baseName = (request.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentException("Folder name is required.", nameof(request.Name));

            if (request.ParentId == Guid.Empty)
                request.ParentId = null;

            if (request.ParentId.HasValue)
            {
                var parentExists = await _dbContext.Folders
                    .AsNoTracking()
                    .AnyAsync(f => f.Id == request.ParentId.Value, cancellationToken);

                if (!parentExists)
                    request.ParentId = null;
            }

            var siblingNames = await _dbContext.Folders
                .AsNoTracking()
                .Where(f => f.ParentId == request.ParentId)
                .Select(f => f.Name)
                .ToListAsync(cancellationToken);

            var used = new HashSet<string>(siblingNames, StringComparer.OrdinalIgnoreCase);

            var finalName = baseName;
            var i = 1;
            while (used.Contains(finalName))
                finalName = $"{baseName} ({i++})";

            var folder = new Domain.Entities.Folder
            {
                Id = Guid.NewGuid(),
                Name = finalName,
                ParentId = request.ParentId,
                CreatedOn = DateTimeOffset.UtcNow,
                ModifiedOn = DateTimeOffset.UtcNow,
                EntityStatus = Common.Enums.eEntityStatus.Active,
            };

            _dbContext.Folders.Add(folder);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return folder.Id;
        }
    }
}
