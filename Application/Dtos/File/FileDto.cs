using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.File
{
    public class FileDto
    {
        public Guid Id { get; set; }
        public Guid? FolderId { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long SizeInBytes { get; set; }
        public byte[] Data { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
    }
}
