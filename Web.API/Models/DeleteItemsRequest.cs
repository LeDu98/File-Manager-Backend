namespace Web.API.Models
{
    public class DeleteItemsRequest
    {
        public List<Guid> FileIds { get; set; }
        public List<Guid> FolderIds { get; set; }
    }
}
