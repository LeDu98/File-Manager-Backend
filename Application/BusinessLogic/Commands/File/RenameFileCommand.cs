using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BusinessLogic.Commands.File
{
    public class RenameFileCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class RenameFileCommandHandler : IRequestHandler<RenameFileCommand, Guid>
    {
        private readonly ApplicationDbContext _dbContext;
        public RenameFileCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Guid> Handle(RenameFileCommand request, CancellationToken ct)
        {
            var baseFromClient = (request.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(baseFromClient))
                throw new ArgumentException("File name is required.", nameof(request.Name));

            if (request.Id == Guid.Empty)
                throw new ArgumentException(nameof(request.Id));

            var file = await _dbContext.Files
                .FirstOrDefaultAsync(f => f.Id == request.Id, ct);

            if (file is null)
                throw new KeyNotFoundException("File was not found.");

            var currentExt = System.IO.Path.GetExtension(file.Name);
            var desiredFullName = baseFromClient + currentExt;

            var usedFullNames = await _dbContext.Files
                .AsNoTracking()
                .Where(f => f.FolderId == file.FolderId && f.Id != file.Id)
                .Select(f => f.Name)
                .ToListAsync(ct);

            var used = new HashSet<string>(usedFullNames, StringComparer.OrdinalIgnoreCase);

            if (string.Equals(file.Name, desiredFullName, StringComparison.OrdinalIgnoreCase))
                return file.Id;

            var uniqueFullName = MakeUniqueFromBase(baseFromClient, currentExt, used);

            file.Name = uniqueFullName;
            file.ModifiedOn = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync(ct);
            return file.Id;
        }

        private static string MakeUniqueFromBase(string baseName, string ext, ISet<string> used)
        {
            var trimmedBase = baseName.Trim();

            var match = System.Text.RegularExpressions.Regex.Match(trimmedBase, @"^(.*)\((\d+)\)$");
            string core = trimmedBase;
            int start = 1;

            if (match.Success)
            {
                core = match.Groups[1].Value.TrimEnd();
                if (int.TryParse(match.Groups[2].Value, out var n)) start = Math.Max(1, n);
            }

            var candidate = $"{core}{ext}";
            if (!used.Contains(candidate))
                return candidate;

            var i = start;
            do
            {
                candidate = $"{core}({i++}){ext}";
            } while (used.Contains(candidate));

            return candidate;
        }
    }
}
