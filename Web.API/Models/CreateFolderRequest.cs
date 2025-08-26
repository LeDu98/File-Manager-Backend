namespace Web.API.Models
{
    public class CreateFolderRequest
    {
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
    }
}
