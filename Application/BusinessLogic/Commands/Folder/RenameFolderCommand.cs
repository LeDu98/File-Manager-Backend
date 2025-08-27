using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BusinessLogic.Commands.Folder
{
    public class RenameFolderCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class RenameFolderCommandHandler : IRequestHandler<RenameFolderCommand, Guid>
    {
        private readonly ApplicationDbContext _dbContext;
        public RenameFolderCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Guid> Handle(RenameFolderCommand request, CancellationToken cancellationToken)
        {
            var baseName = (request.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentException("Folder name is required.", nameof(request.Name));

            if (request.Id == Guid.Empty)
                throw new ArgumentException(nameof(request.Id));

            var folder = await _dbContext.Folders.SingleOrDefaultAsync(_ => _.Id == request.Id);
            if (folder == null)
            {
                throw new ArgumentException("Folder was not found!");
            }

            var siblingNames = await _dbContext.Folders
                .AsNoTracking()
                .Where(f => f.ParentId == folder.ParentId)
                .Select(f => f.Name)
                .ToListAsync(cancellationToken);

            var used = new HashSet<string>(siblingNames, StringComparer.OrdinalIgnoreCase);

            var finalName = baseName;
            var i = 1;
            while (used.Contains(finalName))
                finalName = $"{baseName} ({i++})";

            folder.Name = finalName;
            folder.ModifiedOn = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return folder.Id;
        }
    }
}
