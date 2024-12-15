using prometeyapi.Core.Enums;

namespace prometeyapi.Core.DTOs.UserDTOs.Request;

public record UpdateUserRequestDTO
{
    public string base64ProfilePhoto { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Username { get; set; }
    public Category Category { get; set; }
}
