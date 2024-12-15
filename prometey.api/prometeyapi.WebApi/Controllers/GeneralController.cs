using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prometeyapi.Core.DTOs.ApplicationDTOs.Response;
using prometeyapi.Core.DTOs.GeneralDTOs.Response;
using prometeyapi.Core.DTOs.GroupDTOs.Response;
using prometeyapi.Core.DTOs.PostDTOs.Response;
using prometeyapi.Core.Enums;
using prometeyapi.Core.Models;
using prometeyapi.Infrastructure.Data;

namespace prometeyapi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly DBContext _dbContext;

        public GeneralController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GENERAL

        /// <summary>
        /// Search posts and applications
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("search/{search}")]
        public async Task<IActionResult> Search(string search)
        {
            var posts = await _dbContext.Posts.AsNoTracking().Where(x => x.Title.Contains(search) || x.Description.Contains(search) || x.Content.Contains(search)).ToListAsync();
            var applications = await _dbContext.Applications.AsNoTracking().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToListAsync();
            var users = await _dbContext.Users.AsNoTracking().Where(x => x.Name.Contains(search) || x.Surname.Contains(search) || x.Username.Contains(search)).ToListAsync();
            var groups = await _dbContext.Groups.AsNoTracking().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToListAsync();

            var res_posts = posts.Select(p => new GetPostResponseDTO
            {
                Id = p.Id,
                AuthorUsername = p.AuthorUsername,
                PostPhotoLink = p.PostPhotoLink,
                Title = p.Title,
                Description = p.Description,
                Content = p.Content,
                Category = p.Category,
                Author = p.Author,
                CreatedAt = p.CreatedAt
            });

            var res_applications = applications.Select(a => new GetApplicationResponseDTO
            {
                Id = a.Id,
                AuthorUsername = a.AuthorUsername,
                Name = a.Name,
                Description = a.Description,
                Category = a.Category,
                Created = a.Created
            });

            var res_users = users.Select(u => new GetUserDetailsResponseDTO
            {
                ProfilePhotoLink = u.ProfilePhotoLink,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Username = u.Username,
                Category = u.Category,
            });

            var res_groups = groups.Select(g => new GetGroupResponseDTO
            {
                Name = g.Name,
                Description = g.Description,
            });

            return Ok(new { posts = res_posts.ToList(), applications = res_applications.ToList() });
        }

        // USERS

        /// <summary>
        /// Get user details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-user-details/{username}")]
        public async Task<GetUserDetailsResponseDTO> GetUserDetails(string username)
        {
            User user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username == username);

            GetUserDetailsResponseDTO res = new GetUserDetailsResponseDTO
            {
                Id = user.Id,
                ProfilePhotoLink = user.ProfilePhotoLink,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Username = user.Username,
                Category = user.Category,
            };

            return res;
        }

        /// <summary>
        /// Get users by search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("search-users/{search}")]
        public async Task<List<GetUserDetailsResponseDTO>> SearchUsers(string search)
        {
            var users = await _dbContext.Users.Where(x => x.Name.Contains(search) || x.Surname.Contains(search) || x.Username.Contains(search)).ToListAsync();
            var res_users = users.Select(u => new GetUserDetailsResponseDTO
            {
                ProfilePhotoLink = u.ProfilePhotoLink,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Username = u.Username,
                Category = u.Category,
            });

            return res_users.ToList();
        }

        // POSTS

        /// <summary>
        /// Get post by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-post-by-id/{id}")]
        public async Task<GetPostResponseDTO> GetPostById(Guid id)
        {
            Post post = await _dbContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

            GetPostResponseDTO res = new GetPostResponseDTO
            {
                Id = post.Id,
                AuthorUsername = post.AuthorUsername,
                PostPhotoLink = post.PostPhotoLink,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Category = post.Category,
                Author = post.Author,
                CreatedAt = post.CreatedAt
            };

            return res;
        }

        /// <summary>
        /// Get posts with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("get-posts-pagination")]
        public async Task<IActionResult> GetPostsPagination(int page, int pageSize = 6)
        {
            var totalPosts = await _dbContext.Posts.CountAsync();
            var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            var posts = await _dbContext.Posts.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { posts, totalPages });
        }

        /// <summary>
        /// Get posts with pagination by user Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("get-posts-pagination-by-user-id/{id}")]
        public async Task<IActionResult> GetPostsPaginationByUserId(Guid id, int page, int pageSize = 6)
        {
            var totalPosts = await _dbContext.Posts.Where(x => x.UserId == id).CountAsync();
            var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            var posts = await _dbContext.Posts.AsNoTracking()
                .Where(predicate => predicate.UserId == id)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { posts, totalPages });
        }

        /// <summary>
        /// Get posts by category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("get-posts-by-category/{category}")]
        public async Task<List<GetPostResponseDTO>> GetPostsByCategory(Category category)
        {
            var posts = await _dbContext.Posts.Where(x => x.Category == category).ToListAsync();
            var res_posts = posts.Select(p => new GetPostResponseDTO
            {
                Id = p.Id,
                AuthorUsername = p.AuthorUsername,
                PostPhotoLink = p.PostPhotoLink,
                Title = p.Title,
                Description = p.Description,
                Content = p.Content,
                Category = p.Category,
                Author = p.Author,
                CreatedAt = p.CreatedAt
            });

            return res_posts.ToList();
        }

        // APPLICATIONS

        /// <summary>
        /// Get application by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-application-by-id/{id}")]
        public async Task<GetApplicationResponseDTO> GetApplicationById(Guid id)
        {
            Application application = await _dbContext.Applications.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

            GetApplicationResponseDTO res = new GetApplicationResponseDTO
            {
                Id = application.Id,
                AuthorUsername = application.AuthorUsername,
                ApplicationPhotoLink = application.ApplicationPhotoLink,
                Name = application.Name,
                Description = application.Description,
                Category = application.Category,
                ApplicationFileUrl = application.ApplicationFileUrl,
                Author = application.Author,
                Created = application.Created
            };

            return res;
        }

        /// <summary>
        /// Get applications
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("get-applications-pagination")]
        public async Task<IActionResult> GetApplicationsPagination(int page, int pageSize = 1)
        {
            var totalApplications = await _dbContext.Applications.CountAsync();
            var totalPages = (int)Math.Ceiling(totalApplications / (double)pageSize);
            var applications = await _dbContext.Applications.AsNoTracking()
                .Where(x => x.IsVerified == true)
                .OrderByDescending(x => x.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new GetApplicationResponseDTO
                {
                    Id = a.Id,
                    Author = a.Author,
                    AuthorUsername = a.AuthorUsername,
                    ApplicationPhotoLink = a.ApplicationPhotoLink,
                    Name = a.Name,
                    Description = a.Description,
                    Category = a.Category,
                    ApplicationFileUrl = a.ApplicationFileUrl,
                    Created = a.Created
                })
                .ToListAsync();

            return Ok(new { applications, totalPages });
        }

        /// <summary>
        /// Get applications by user Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("get-applications-pagination-by-user-id/{id}")]
        public async Task<IActionResult> GetApplicationsPaginationByUserId(Guid id, int page, int pageSize = 1)
        {
            var totalApplications = await _dbContext.Applications.Where(x => x.UserId == id).CountAsync();
            var totalPages = (int)Math.Ceiling(totalApplications / (double)pageSize);
            var applications = await _dbContext.Applications.AsNoTracking()
                .Where(predicate => predicate.IsVerified == true && predicate.UserId == id)
                .OrderByDescending(x => x.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new GetApplicationResponseDTO
                {
                    Id = a.Id,
                    AuthorUsername = a.AuthorUsername,
                    ApplicationPhotoLink = a.ApplicationPhotoLink,
                    Name = a.Name,
                    Description = a.Description,
                    Category = a.Category,
                    ApplicationFileUrl = a.ApplicationFileUrl,
                    Created = a.Created
                })
                .ToListAsync();

            return Ok(new { applications, totalPages });
        }

        /// <summary>
        /// Get applications by category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("get-applications-by-category/{category}")]
        public async Task<List<GetApplicationResponseDTO>> GetApplicationsByCategory(Category category)
        {
            var applications = await _dbContext.Applications.Where(x => x.Category == category && x.IsVerified == true).ToListAsync();
            var res_applications = applications.Select(a => new GetApplicationResponseDTO
            {
                Id = a.Id,
                AuthorUsername = a.AuthorUsername,
                Name = a.Name,
                Description = a.Description,
                Category = a.Category,
                ApplicationFileUrl = a.ApplicationFileUrl,
                Created = a.Created
            });

            return res_applications.ToList();
        }
    }
}
