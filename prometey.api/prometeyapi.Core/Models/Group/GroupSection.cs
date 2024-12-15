namespace prometeyapi.Core.Models.Group;

public class GroupSection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string GroupPhotoLink { get; set; }
    public string Domain { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<User> Members { get; set; } = new List<User>();
    public ICollection<GroupPost> Posts { get; set; } = new List<GroupPost>();
    public ICollection<GroupApplication> Applications { get; set; } = new List<GroupApplication>();
    public Guid UserId { get; set; }
    public User User { get; set; }
}