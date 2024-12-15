using prometeyapi.Core.Enums;

namespace prometeyapi.Core.Models.Group;

public class GroupPost
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string GroupPostPhotoLink { get; set; } = string.Empty;
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public Category Category { get; set; } = Category.NONE;
    public string Author { get; set; }
    public string AuthorUsername { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid GroupId { get; set; }
    public GroupSection Group { get; set; }
}