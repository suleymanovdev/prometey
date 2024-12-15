using prometeyapi.Core.Enums;

namespace prometeyapi.Core.DTOs.UserDTOs.Response;

public record GetUserResponseDTO
{
    public string ProfilePhotoLink { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Username { get; set; }
    public Category Category { get; set; }
}