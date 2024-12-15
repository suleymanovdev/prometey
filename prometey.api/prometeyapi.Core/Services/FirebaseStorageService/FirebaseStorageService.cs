using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace prometeyapi.Core.Services.FirebaseStorageService;

public class FirebaseStorageService
{
    private const string ProjectId = "prometey-b3e55";
    private const string ServiceAccountKeyFilePath = "prometey-b3e55-9f5707be6a44.json";

    // PHOTOS

    // PROFILE PHOTO

    public async Task UploadProfilePhotoAsync(Guid userId, string base64ProfilePhoto)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"profile-photos/{userId}.jpg";

        var imageBytes = Convert.FromBase64String(base64ProfilePhoto);
        using var memoryStream = new MemoryStream(imageBytes);

        await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);

        memoryStream.Close();
    }

    public async Task<string> GetProfilePhotoUrlAsync(Guid userId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"profile-photos/{userId}.jpg";

        var storageFilePath = $"profile-photos/{userId}.jpg";

        string downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(storageFilePath)}?alt=media";

        return downloadUrl;
    }

    public async Task DeleteProfilePhotoAsync(Guid userId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"profile-photos/{userId}.jpg";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }

    // POST PHOTO

    public async Task UploadPostPhotoAsync(Guid postId, string base64PostPhoto)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"post-photos/{postId}.jpg";

        var imageBytes = Convert.FromBase64String(base64PostPhoto);
        using var memoryStream = new MemoryStream(imageBytes);

        await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);

        memoryStream.Close();
    }

    public async Task<string> GetPostPhotoUrlAsync(Guid postId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"post-photos/{postId}.jpg";

        var storageFilePath = $"post-photos/{postId}.jpg";

        string downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(storageFilePath)}?alt=media";

        return downloadUrl;
    }

    public async Task DeletePostPhotoAsync(Guid postId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"post-photos/{postId}.jpg";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }

    // APPLICATION PHOTO

    public async Task UploadApplicationPhotoAsync(Guid applicationId, string base64ApplicationPhoto)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"application-photos/{applicationId}.jpg";

        var imageBytes = Convert.FromBase64String(base64ApplicationPhoto);
        using var memoryStream = new MemoryStream(imageBytes);

        await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);

        memoryStream.Close();
    }

    public async Task<string> GetApplicationPhotoUrlAsync(Guid applicationId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"application-photos/{applicationId}.jpg";

        var storageFilePath = $"application-photos/{applicationId}.jpg";

        string downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(storageFilePath)}?alt=media";

        return downloadUrl;
    }

    public async Task DeleteApplicationPhotoAsync(Guid applicationId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"application-photos/{applicationId}.jpg";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }

    // GROUP PHOTO

    public async Task UploadGroupPhotoAsync(Guid groupId, string base64GroupPhoto)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-photos/{groupId}.jpg";

        var imageBytes = Convert.FromBase64String(base64GroupPhoto);
        using var memoryStream = new MemoryStream(imageBytes);

        await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);

        memoryStream.Close();
    }

    public async Task<string> GetGroupPhotoUrlAsync(Guid groupId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-photos/{groupId}.jpg";

        var storageFilePath = $"group-photos/{groupId}.jpg";

        string downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(storageFilePath)}?alt=media";

        return downloadUrl;
    }

    public async Task DeleteGroupPhotoAsync(Guid groupId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-photos/{groupId}.jpg";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }

    // GROUP POST PHOTO

    public async Task UploadGroupPostPhotoAsync(Guid groupPostId, string base64GroupPostPhoto)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-post-photos/{groupPostId}.jpg";

        var imageBytes = Convert.FromBase64String(base64GroupPostPhoto);
        using var memoryStream = new MemoryStream(imageBytes);

        await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);

        memoryStream.Close();
    }

    public async Task<string> GetGroupPostPhotoUrlAsync(Guid groupPostId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-post-photos/{groupPostId}.jpg";

        var storageFilePath = $"group-post-photos/{groupPostId}.jpg";

        string downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(storageFilePath)}?alt=media";

        return downloadUrl;
    }

    public async Task DeleteGroupPostPhotoAsync(Guid groupPostId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-post-photos/{groupPostId}.jpg";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }

    // GROUP APPLICATION PHOTO

    public async Task UploadGroupApplicationPhotoAsync(Guid groupApplicationId, string base64GroupApplicationPhoto)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-application-photos/{groupApplicationId}.jpg";

        var imageBytes = Convert.FromBase64String(base64GroupApplicationPhoto);
        using var memoryStream = new MemoryStream(imageBytes);

        await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);

        memoryStream.Close();
    }

    public async Task<string> GetGroupApplicationPhotoUrlAsync(Guid groupApplicationId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-application-photos/{groupApplicationId}.jpg";

        var storageFilePath = $"group-application-photos/{groupApplicationId}.jpg";

        string downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(storageFilePath)}?alt=media";

        return downloadUrl;
    }

    public async Task DeleteGroupApplicationPhotoAsync(Guid groupApplicationId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-application-photos/{groupApplicationId}.jpg";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }

    // APPLICATIONS

    // APPLICATION FILE

    public async Task UploadApplicationFileAsync(Guid applicationId, string base64ApplicationFile)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"application-files/{applicationId}.zip";

        var fileBytes = Convert.FromBase64String(base64ApplicationFile);
        using var memoryStream = new MemoryStream(fileBytes);

        await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);

        memoryStream.Close();
    }

    public async Task<string> GetApplicationFileUrlAsync(Guid applicationId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"application-files/{applicationId}.zip";

        var storageFilePath = $"application-files/{applicationId}.zip";

        string downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(storageFilePath)}?alt=media";

        return downloadUrl;
    }

    public async Task DeleteApplicationFileAsync(Guid applicationId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"application-files/{applicationId}.zip";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }

    // GROUP APPLICATION FILE

    public async Task UploadGroupApplicationFileAsync(Guid groupApplicationId, string base64GroupApplicationFile)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-application-files/{groupApplicationId}.zip";

        var fileBytes = Convert.FromBase64String(base64GroupApplicationFile);
        using var memoryStream = new MemoryStream(fileBytes);

        await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);

        memoryStream.Close();
    }

    public async Task<string> GetGroupApplicationFileUrlAsync(Guid groupApplicationId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-application-files/{groupApplicationId}.zip";

        var storageFilePath = $"group-application-files/{groupApplicationId}.zip";

        string downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(storageFilePath)}?alt=media";

        return downloadUrl;
    }

    public async Task DeleteGroupApplicationFileAsync(Guid groupApplicationId)
    {
        var credential = GoogleCredential.FromFile(ServiceAccountKeyFilePath);
        var storage = StorageClient.Create(credential);

        var bucketName = "prometey-b3e55.appspot.com";
        var objectName = $"group-application-files/{groupApplicationId}.zip";

        await storage.DeleteObjectAsync(bucketName, objectName);
    }
}