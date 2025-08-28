using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FileContent
    {
        public Guid FileId { get; set; }
        public byte[] Data { get; set; }
        public File File { get; set; }
    }
}
