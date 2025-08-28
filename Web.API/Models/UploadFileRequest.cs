namespace Web.API.Models
{
    public class UploadFileRequest
    {
        public List<IFormFile> Files { get; set; }
        public Guid? ParentId { get; set; }
    }
}
