using Microsoft.EntityFrameworkCore;
using prometeyapi.Core.DTOs.ApplicationDTOs.Request;
using prometeyapi.Core.DTOs.ApplicationDTOs.Response;
using prometeyapi.Core.Exceptions.ApplicationExceptions;
using prometeyapi.Core.Models;
using prometeyapi.Core.Services.FirebaseStorageService;
using prometeyapi.Infrastructure.Data;
using prometeyapi.Infrastructure.Services.ImageService;
using System.IO.Compression;

namespace prometeyapi.Infrastructure.Services.ApplicationServices;

public class ApplicationService
{
    private FirebaseStorageService firebaseStorageService = new();

    public async Task<Application> CreateApplication(DBContext dbContext, CreateApplicationRequestDTO request)
    {
        try
        {
            var user = await dbContext.Users.FindAsync(request.userId);

            if (user == null)
            {
                throw new CreateApplicationRequestException("User not found.");
            }

            if (request == null)
            {
                throw new CreateApplicationRequestException("Request is null.");
            }

            if (request.file == null)
            {
                throw new CreateApplicationRequestException("File is null.");
            }

            if (!request.file.FileName.EndsWith(".zip"))
            {
                throw new CreateApplicationRequestException("File is not a zip archive.");
            }

            Application application = new();

            var imageProcessor = new ImageProcessor();
            string processedImage = await imageProcessor.ProcessPostOrApplicationPhotoAsync(request.base64ApplicationPhoto);

            using (var memoryStream = new MemoryStream())
            {
                await request.file.CopyToAsync(memoryStream);
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
                {
                    var zipContent = Convert.ToBase64String(memoryStream.ToArray());
                    await firebaseStorageService.UploadApplicationFileAsync(application.Id, zipContent);
                }

                await firebaseStorageService.UploadApplicationPhotoAsync(application.Id, processedImage);

                application.ApplicationPhotoLink = await firebaseStorageService.GetApplicationPhotoUrlAsync(application.Id);
                application.Name = request.Name;
                application.Description = request.Description;
                application.Category = request.Category;
                application.Author = $"{user.Name} {user.Surname}";
                application.AuthorUsername = user.Username;
                application.ApplicationFileUrl = await firebaseStorageService.GetApplicationFileUrlAsync(application.Id);
                application.UserId = request.userId;

                dbContext.Applications.Add(application);
                await dbContext.SaveChangesAsync();
            }

            return application;
        }
        catch (Exception ex)
        {
            throw new CreateApplicationRequestException(ex.Message);
        }
    }

    public async Task<IEnumerable<GetApplicationResponseDTO>> GetApplications(DBContext dbContext, Guid userId)
    {
        try
        {
            var user = await dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new GetApplicationsRequestException("User not found.");
            }

            var applications = dbContext.Applications.Where(a => a.UserId == userId).ToList();

            IEnumerable<GetApplicationResponseDTO> res_applications = applications.Select(a => new GetApplicationResponseDTO
            {
                Id = a.Id,
                ApplicationPhotoLink = a.ApplicationPhotoLink,
                Name = a.Name,
                Description = a.Description,
                Category = a.Category,
                Author = a.Author
            });

            return res_applications;
        }
        catch (Exception ex)
        {
            throw new GetApplicationsRequestException("Error getting applications.", ex);
        }
    }

    public async Task<GetApplicationResponseDTO> GetApplication(DBContext dbContext, Guid userId, Guid applicationId)
    {
        try
        {
            var application = await dbContext.Applications.FindAsync(applicationId);

            if (application == null)
            {
                throw new GetApplicationRequestException("Application not found.");
            }

            if (application.UserId != userId)
            {
                throw new GetApplicationRequestException("Application not found.");
            }

            GetApplicationResponseDTO res_application = new GetApplicationResponseDTO
            {
                Id = application.Id,
                ApplicationPhotoLink = application.ApplicationPhotoLink,
                Name = application.Name,
                Description = application.Description,
                Category = application.Category,
                Author = application.Author
            };

            return res_application;
        }
        catch (Exception ex)
        {
            throw new GetApplicationRequestException("Error getting application.", ex);
        }
    }

    public async Task DeleteApplication(DBContext dbContext, Guid userId, Guid applicationId)
    {
        try
        {
            Application application = await dbContext.Applications.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == applicationId);

            if (application == null)
            {
                throw new DeleteApplicationRequestException("Application not found.");
            }

            await firebaseStorageService.DeleteApplicationPhotoAsync(application.Id);
            await firebaseStorageService.DeleteApplicationFileAsync(application.Id);

            dbContext.Applications.Remove(application);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DeleteApplicationRequestException("Error deleting application.", ex);
        }
    }
}