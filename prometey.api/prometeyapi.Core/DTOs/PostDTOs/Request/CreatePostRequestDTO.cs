using prometeyapi.Core.Enums;

namespace prometeyapi.Core.DTOs.PostDTOs.Request;

public record CreatePostRequestDTO
{
    public string base64PostPhoto { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public Category Category { get; set; }
}