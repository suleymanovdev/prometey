using prometeyapi.Core.Enums;

namespace prometeyapi.Core.Models.Group;

public class GroupApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string GroupApplicationPhotoLink { get; set; } = string.Empty;
    public string Name { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; } = Category.NONE;
    public string Author { get; set; }
    public string AuthorUsername { get; set; }
    public string ApplicationFileUrl { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public bool IsVerified { get; set; } = false;
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid GroupId { get; set; }
    public GroupSection Group { get; set; }
}