using prometeyapi.Core.Enums;

namespace prometeyapi.Core.DTOs.AuthDTOs.Request;

public record SignUpRequestDTO
{
    public string base64ProfilePhoto { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Username { get; set; }
    public Category Category { get; set; }
}