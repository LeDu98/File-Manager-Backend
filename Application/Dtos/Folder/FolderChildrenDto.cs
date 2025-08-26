using Application.Dtos.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Folder
{
    public class FolderChildrenDto
    {
        public Guid? FolderId { get; set; }
        public List<FolderDto> Folders { get; set; }
        public List<FileDto> Files { get; set; }
    }
}
