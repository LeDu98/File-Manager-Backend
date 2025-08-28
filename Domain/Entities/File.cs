using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class File
    {
        public Guid Id { get; set; }
        public eEntityStatus EntityStatus { get; set; }
        public Guid? FolderId { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long SizeInBytes { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public Folder Folder { get; set; }
        public FileContent Content { get; set; }
    }
}
