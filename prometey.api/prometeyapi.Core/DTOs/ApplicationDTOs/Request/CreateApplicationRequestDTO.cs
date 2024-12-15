using prometeyapi.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace prometeyapi.Core.DTOs.ApplicationDTOs.Request;

public record CreateApplicationRequestDTO
{
    public Guid userId { get; set; }
    public string base64ApplicationPhoto { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
    public IFormFile file { get; set; }
}