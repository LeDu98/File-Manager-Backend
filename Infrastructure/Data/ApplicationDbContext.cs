using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Domain.Entities.File>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(255).IsRequired();
                e.Property(x => x.ContentType).HasMaxLength(150);
                e.Property(x => x.SizeInBytes).IsRequired();

                e.HasOne(x => x.Folder)
                 .WithMany(f => f.Files)
                 .HasForeignKey(x => x.FolderId);

                e.HasOne(x => x.Content)
                 .WithOne(c => c.File)
                 .HasForeignKey<FileContent>(c => c.FileId)
                 .IsRequired();
            });

            modelBuilder.Entity<FileContent>(e =>
            {
                e.HasKey(x => x.FileId);
                e.Property(x => x.Data).IsRequired();
            });
        }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<FileContent> FileContents { get; set; }
    }
}
