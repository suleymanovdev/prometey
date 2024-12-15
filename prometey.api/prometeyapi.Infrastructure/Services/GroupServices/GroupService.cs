using Microsoft.EntityFrameworkCore;
using prometeyapi.Core.DTOs.GroupDTOs.Request;
using prometeyapi.Core.DTOs.GroupDTOs.Response;
using prometeyapi.Core.Exceptions.ApplicationExceptions;
using prometeyapi.Core.Exceptions.GroupExceptions;
using prometeyapi.Core.Models.Group;
using prometeyapi.Core.Services.FirebaseStorageService;
using prometeyapi.Infrastructure.Data;
using prometeyapi.Infrastructure.Services.ImageService;

namespace prometeyapi.Infrastructure.Services.GroupServices;

public class GroupService
{
    public async Task CreateGroup(DBContext dbContext, Guid userId, CreateGroupRequestDTO request)
    {
        if (request == null) {
            throw new CreateApplicationRequestException("Request is null.");
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            throw new CreateApplicationRequestException("Name is null or empty.");
        }

        if (string.IsNullOrEmpty(request.Description))
        {
            throw new CreateApplicationRequestException("Description is null or empty.");
        }

        if (string.IsNullOrEmpty(request.Logo))
        {
            throw new CreateApplicationRequestException("Logo is null or empty.");
        }

        if (string.IsNullOrEmpty(request.Domain))
        {
            throw new CreateApplicationRequestException("Domain is null or empty.");
        }

        GroupSection checkGroup = await dbContext.Groups.FirstOrDefaultAsync(x => x.Domain == request.Domain);

        if (checkGroup != null) {
            throw new CreateApplicationRequestException("Group with this domain already exists.");
        }

        User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        ImageProcessor imageService = new ImageProcessor();

        string processedImage = await imageService.ProcessGroupPhotoAsync(request.Logo);

        FirebaseStorageService firebaseStorageService = new();

        GroupSection group = new GroupSection
        {
            Name = request.Name,
            Description = request.Description,
            Domain = request.Domain,
            UserId = user.Id            
        };

        group.Members.Add(user);

        await firebaseStorageService.UploadGroupPhotoAsync(group.Id, processedImage);

        group.GroupPhotoLink = await firebaseStorageService.GetGroupPhotoUrlAsync(group.Id);

        dbContext.Groups.Add(group);
        await dbContext.SaveChangesAsync();
    }

    public async Task<GetGroupResponseDTO> GetGroup(DBContext dbContext, string domain)
    {
        GroupSection group = await dbContext.Groups.FirstOrDefaultAsync(x => x.Domain == domain);

        if (group == null)
        {
            throw new GetGroupRequestException("Group not found.");
        }

        return new GetGroupResponseDTO
        {
            GroupPhotoLink = group.GroupPhotoLink,
            Name = group.Name,
            Description = group.Description,
            Domain = group.Domain,
            CreatedAt = group.CreatedAt
        };
    }

    public async Task<ICollection<GetGroupResponseDTO>> GetGroups(DBContext dbContext, Guid userId)
    {
        ICollection<GroupSection> groups = await dbContext.Groups.Where(dbContext => dbContext.Members.Any(x => x.Id == userId)).ToListAsync();

        if (groups == null)
        {
            throw new GetGroupRequestException("Groups not found.");
        }

        ICollection<GetGroupResponseDTO> response = new List<GetGroupResponseDTO>();

        foreach (GroupSection group in groups)
        {
            response.Add(new GetGroupResponseDTO
            {
                GroupPhotoLink = group.GroupPhotoLink,
                Name = group.Name,
                Description = group.Description,
                Domain = group.Domain
            });
        }

        return response;
    }
}