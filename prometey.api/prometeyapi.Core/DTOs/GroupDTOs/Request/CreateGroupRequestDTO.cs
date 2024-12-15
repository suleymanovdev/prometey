namespace prometeyapi.Core.DTOs.GroupDTOs.Request;

public record CreateGroupRequestDTO
{
    public string Logo { get; set; }
    public string Domain { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}