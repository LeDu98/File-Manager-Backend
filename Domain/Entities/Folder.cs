using Common.Enums;

namespace Domain.Entities
{
    public class Folder
    {
        public Guid Id { get; set; }
        public eEntityStatus EntityStatus { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public Folder Parent { get; set; }
        public ICollection<Folder> Children { get; set; }
        public ICollection<File> Files { get; set; }
    }
}
