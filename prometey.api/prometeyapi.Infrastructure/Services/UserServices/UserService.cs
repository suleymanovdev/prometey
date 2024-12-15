using Microsoft.EntityFrameworkCore;
using prometeyapi.Core.DTOs.UserDTOs.Request;
using prometeyapi.Core.DTOs.UserDTOs.Response;
using prometeyapi.Core.Exceptions.UserExceptions;
using prometeyapi.Core.Services.FirebaseStorageService;
using prometeyapi.Infrastructure.Data;
using prometeyapi.Infrastructure.Services.ImageService;

namespace prometeyapi.Infrastructure.Services.UserServices;

public class UserService
{
    public async Task<GetUserResponseDTO> GetUser(DBContext dbContext, Guid id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user == null)
        {
            throw new GetUserRequestException("User not found.");
        }

        GetUserResponseDTO res = new GetUserResponseDTO
        {
            ProfilePhotoLink = user.ProfilePhotoLink,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Username = user.Username,
            Category = user.Category
        };

        return res;
    }

    public async Task<bool> UpdateUser(DBContext dbContext, Guid id, UpdateUserRequestDTO request)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user == null)
        {
            throw new UpdateUserRequestException("User not found.");
        }

        if (!string.IsNullOrEmpty(request.base64ProfilePhoto))
        {
            var imageProcessor = new ImageProcessor();
            string processedPhoto = await imageProcessor.ProcessProfilePhotoAsync(request.base64ProfilePhoto);
            if (processedPhoto != null)
            {
                FirebaseStorageService firebaseStorageService = new FirebaseStorageService();
                await firebaseStorageService.UploadProfilePhotoAsync(user.Id, processedPhoto);
                user.ProfilePhotoLink = await firebaseStorageService.GetProfilePhotoUrlAsync(user.Id);
            }
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            user.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            user.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Surname))
        {
            user.Surname = request.Surname;
        }

        if (!string.IsNullOrEmpty(request.Username))
        {
            var findUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username);

            if (findUser != null && findUser != user)
            {
                throw new UpdateUserRequestException("User with this email already exists.");
            }

            user.Username = request.Username;
        }

        if (request.Category != null)
        {
            user.Category = request.Category;
        }

        var posts = await dbContext.Posts.Where(x => x.UserId == id).ToListAsync();
        var applications = await dbContext.Applications.Where(x => x.UserId == id).ToListAsync();

        foreach (var post in posts)
        {
            post.Author = user.Name + " " + user.Surname;
            post.AuthorUsername = user.Username;
        }

        foreach (var application in applications)
        {
            application.Author = user.Name + " " + user.Surname;
            application.AuthorUsername = user.Username;
        }

        dbContext.Users.Update(user);
        dbContext.Posts.UpdateRange(posts);
        dbContext.Applications.UpdateRange(applications);

        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task DeleteUser(DBContext dbContext, Guid id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user == null)
        {
            throw new DeleteUserRequestException("User not found.");
        }

        FirebaseStorageService firebaseStorageService = new FirebaseStorageService();

        var posts = await dbContext.Posts.Where(x => x.UserId == id).ToListAsync();
        var applications = await dbContext.Applications.Where(x => x.UserId == id).ToListAsync();

        foreach (var post in posts)
        {
            await firebaseStorageService.DeletePostPhotoAsync(post.Id);
        }

        foreach (var application in applications)
        {
            await firebaseStorageService.DeleteApplicationPhotoAsync(application.Id);
            await firebaseStorageService.DeleteApplicationFileAsync(application.Id);
        }

        await firebaseStorageService.DeleteProfilePhotoAsync(user.Id);

        dbContext.Posts.RemoveRange(posts);
        dbContext.Applications.RemoveRange(applications);
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
    }
}
