namespace prometeyapi.Core.DTOs.AuthDTOs.Request;

public record LogInRequestDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}
