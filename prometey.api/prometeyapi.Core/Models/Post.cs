using prometeyapi.Core.Enums;

namespace prometeyapi.Core.Models;

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PostPhotoLink { get; set; } = string.Empty;
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public Category Category { get; set; } = Category.NONE;
    public string Author { get; set; }
    public string AuthorUsername { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}