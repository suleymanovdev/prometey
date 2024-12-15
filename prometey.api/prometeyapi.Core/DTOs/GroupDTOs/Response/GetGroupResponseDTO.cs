using prometeyapi.Core.DTOs.GroupDTOs.Application.Response;
using prometeyapi.Core.DTOs.GroupDTOs.Post.Response;
using prometeyapi.Core.DTOs.UserDTOs.Response;

namespace prometeyapi.Core.DTOs.GroupDTOs.Response;

public record GetGroupResponseDTO
{
    public string GroupPhotoLink { get; init; }
    public string Domain { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public ICollection<GetUserResponseDTO> Members { get; init; }
    public ICollection<GetGroupPostResponseDTO> Posts { get; init; }
    public ICollection<GetGroupApplicationDTO> Applications { get; init; }
    public Guid UserId { get; init; }
}