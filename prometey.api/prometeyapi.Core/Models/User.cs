using prometeyapi.Core.Enums;
using prometeyapi.Core.Models.Group;
using prometeyapi.Core.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ProfilePhotoLink { get; set; } = string.Empty;
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public Category Category { get; set; } = Category.NONE;
    public Status Status { get; set; } = Status.BASIC;

    // POSTS
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    // APPLICATIONS
    public ICollection<Application> Applications { get; set; } = new List<Application>();

    // GROUP MEMBERSHIP (MANY-TO-MANY)
    public ICollection<GroupSection> MemberOfGroups { get; set; } = new List<GroupSection>();

    // GROUPS OWNED (ONE-TO-MANY)
    public ICollection<GroupSection> OwnedGroups { get; set; } = new List<GroupSection>();

    // GROUP POSTS
    public ICollection<GroupPost> GroupPosts { get; set; } = new List<GroupPost>();

    // GROUP APPLICATIONS
    public ICollection<GroupApplication> GroupApplications { get; set; } = new List<GroupApplication>();

    // DATA FOR OPERATIONS (BACKGROUNDS)
    public Role Role { get; set; } = Role.USER;
    public bool IsVerified { get; set; } = false;
    public DateTime? RegistrationDate { get; set; }

    public void SetRegistrationDate(DateTime dateTime)
    {
        RegistrationDate = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
    }
}
