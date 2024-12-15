namespace prometeyapi.Core.DTOs.GroupDTOs.Post.Response;

public record GetGroupPostResponseDTO
{
    public string PostPhoto { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Content { get; init; }
    public string Author { get; init; }
    public string AuthorUsername { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid UserId { get; init; }
}
