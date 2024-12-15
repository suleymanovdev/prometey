using Microsoft.Extensions.Hosting;
using prometeyapi.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace prometeyapi.Core.Services.Background;

public class VerificationTimeoutService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
   
    private const string ProjectId = "prometey-b3e55";
    private const string ServiceAccountKeyFilePath = ".\\prometey-b3e55-9f5707be6a44.json";

    public VerificationTimeoutService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckForUnverifiedUsers();
            await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
        }
    }

    private async Task CheckForUnverifiedUsers()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

            var cutoff = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(3));
            var unverifiedUsers = dbContext.Users.Where(u => !u.IsVerified && u.RegistrationDate <= cutoff).ToList();

            if (unverifiedUsers.Any())
            {
                for (int i = 0; i < unverifiedUsers.Count; i++)
                {
                    var user = unverifiedUsers[i];
                    dbContext.Users.Remove(user);
                    await DeleteProfilePhotoAsync(user.Id);
                }
                dbContext.Users.RemoveRange(unverifiedUsers);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    public async Task DeleteProfilePhotoAsync(Guid userId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"profile-photos/{userId}.jpg";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }
}
