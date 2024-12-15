using prometeyapi.Core.Enums;

namespace prometeyapi.Core.DTOs.ApplicationDTOs.Response;

public class GetApplicationResponseDTO
{
    public Guid Id { get; set; }
    public string ApplicationPhotoLink { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
    public string ApplicationFileUrl { get; set; }
    public string Author { get; set; }
    public string AuthorUsername { get; set; }
    public DateTime Created { get; set; }
}