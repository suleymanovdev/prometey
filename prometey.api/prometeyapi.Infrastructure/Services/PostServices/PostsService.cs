using prometeyapi.Core.Models;
using prometeyapi.Infrastructure.Data;
using prometeyapi.Core.Exceptions.PostExceptions;
using prometeyapi.Core.DTOs.PostDTOs.Request;
using prometeyapi.Core.DTOs.PostDTOs.Response;
using prometeyapi.Infrastructure.Services.ImageService;
using prometeyapi.Core.Services.FirebaseStorageService;

namespace prometeyapi.Infrastructure.Services.PostServices;

public class PostsService
{
    public async Task<IEnumerable<GetPostResponseDTO>> GetPosts(DBContext dbContext, Guid userId)
    {
        var user = await dbContext.Users.FindAsync(userId);

        if (user == null)
        {
            throw new GetPostsRequestException("User not found.");
        }

        var posts = dbContext.Posts.Where(p => p.UserId == userId).ToList();

        IEnumerable<GetPostResponseDTO> res_posts = posts.Select(p => new GetPostResponseDTO
        {
            Id = p.Id,
            PostPhotoLink = p.PostPhotoLink,
            Title = p.Title,
            Description = p.Description,
            Content = p.Content,
            Author = p.Author,
            CreatedAt = p.CreatedAt
        });

        return res_posts;
    }

    public async Task<GetPostResponseDTO> GetPost(DBContext dbContext, Guid userId, Guid postId)
    {
        var user = await dbContext.Users.FindAsync(userId);

        if (user == null)
        {
            throw new GetPostRequestException("User not found.");
        }

        var post = await dbContext.Posts.FindAsync(postId);

        if (post == null)
        {
            throw new GetPostRequestException("Post not found.");
        }

        if (post.UserId != userId)
        {
            throw new GetPostRequestException("User is not the owner of the post.");
        }

        var res_post = new GetPostResponseDTO
        {
            Id = post.Id,
            PostPhotoLink = post.PostPhotoLink,
            Title = post.Title,
            Description = post.Description,
            Content = post.Content,
            Author = post.Author,
            CreatedAt = post.CreatedAt
        };

        return res_post;
    }

    public async Task CreatePost(DBContext dbContext, Guid userId, CreatePostRequestDTO request)
    {
        var user = await dbContext.Users.FindAsync(userId);

        if (user == null)
        {
            throw new CreatePostRequestException("User not found.");
        }

        var imageProcessor = new ImageProcessor();
        string processedImage = await imageProcessor.ProcessPostOrApplicationPhotoAsync(request.base64PostPhoto);

        FirebaseStorageService firebaseStorageService = new FirebaseStorageService();

        var post = new Post
        {
            Title = request.Title,
            Description = request.Description,
            Content = request.Content,
            Category = request.Category,
            Author = $"{user.Name} {user.Surname}",
            AuthorUsername = user.Username,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        await firebaseStorageService.UploadPostPhotoAsync(post.Id, processedImage);

        post.PostPhotoLink = await firebaseStorageService.GetPostPhotoUrlAsync(post.Id);

        await dbContext.Posts.AddAsync(post);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeletePost(DBContext dbContext, Guid userId, Guid postId)
    {
        var post = await dbContext.Posts.FindAsync(postId);

        if (post == null)
        {
            throw new DeletePostRequestException("Post not found.");
        }

        if (post.UserId != userId)
        {
            throw new DeletePostRequestException("User is not the owner of the post.");
        }

        FirebaseStorageService firebaseStorageService = new FirebaseStorageService();

        await firebaseStorageService.DeletePostPhotoAsync(post.Id);

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync();
    }
}