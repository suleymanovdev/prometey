namespace prometeyapi.Core.DTOs.GroupDTOs.Application.Response;

public class GetGroupApplicationDTO
{
    public string Logo { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public string AuthorUsername { get; set; }
    public DateTime Created { get; set; }
    public bool IsVerified { get; set; }
    public Guid UserId { get; set; }
    public Guid ContentId { get; set; }
}