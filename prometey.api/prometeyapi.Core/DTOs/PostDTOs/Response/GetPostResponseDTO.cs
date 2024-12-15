using prometeyapi.Core.Enums;

namespace prometeyapi.Core.DTOs.PostDTOs.Response;

public record GetPostResponseDTO
{
    public Guid Id { get; set; }
    public string PostPhotoLink { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public Category Category { get; set; }
    public string Author { get; set; }
    public string AuthorUsername { get; set; }
    public DateTime CreatedAt { get; set; }
}