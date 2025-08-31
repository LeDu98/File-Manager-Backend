using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Application.BusinessLogic.Commands
{
    public class DeleteItemsCommand : IRequest
    {
        public List<Guid>? FolderIds { get; set; }
        public List<Guid>? FileIds { get; set; }
        
    }
    public class DeleteItemsCommandHandler : IRequestHandler<DeleteItemsCommand>
    {
        private readonly ApplicationDbContext _dbContext;
        public DeleteItemsCommandHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public async Task<Unit> Handle(DeleteItemsCommand request, CancellationToken cancellationToken)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            if (request.FileIds?.Any() == true)
            {
                await _dbContext.Files.Where(_ => request.FileIds.Contains(_.Id)).ExecuteDeleteAsync();
            }
            if (request.FolderIds?.Any() == true)
            {

                await _dbContext.Files
                    .Where(f => f.FolderId.HasValue && request.FolderIds.Contains(f.FolderId.Value))
                    .ExecuteDeleteAsync(cancellationToken);
                await _dbContext.Folders.Where(_ => request.FolderIds.Contains(_.Id)).ExecuteDeleteAsync();
            }
            await tx.CommitAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
