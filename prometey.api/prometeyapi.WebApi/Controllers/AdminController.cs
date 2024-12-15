using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prometeyapi.Core.Services.FirebaseStorageService;
using prometeyapi.Infrastructure.Data;
using Serilog;

namespace prometeyapi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DBContext _dbContext;

        public AdminController(DBContext dbContext)
        {
            _dbContext = dbContext;
            Log.Debug("AdminController Initialized.");
        }

        // GET Methods

        /// <summary>
        /// Get statistics
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("get-statistics")]
        public IActionResult GetStatistics()
        {
            try
            {
                int usersCount = _dbContext.Users.Count();
                int postsCount = _dbContext.Posts.Count();
                int applicationsCount = _dbContext.Applications.Count();
                Log.Information("Statistics are fetched by admin.");

                return Ok(new { UsersCount = usersCount, PostsCount = postsCount, ApplicationsCount = applicationsCount });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Get users with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("get-users")]
        public IActionResult GetUsersPagination(int page, int pageSize = 6)
        {
            try
            {
                var users = _dbContext.Users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var totalUsers = _dbContext.Users.Count();
                var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
                Log.Information("Users are fetched by admin.");

                return Ok(new { items = users, totalPages = totalPages });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Get posts with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("get-posts")]
        public IActionResult GetPostsPagination(int page, int pageSize = 6)
        {
            try
            {
                var posts = _dbContext.Posts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var totalPosts = _dbContext.Posts.Count();
                var totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);
                Log.Information("Posts are fetched by admin.");

                return Ok(new { items = posts, totalPages = totalPages });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Get applications with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("get-applications")]
        public IActionResult GetApplicationsPagination(int page, int pageSize = 6)
        {
            try
            {
                var applications = _dbContext.Applications.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var totalApplications = _dbContext.Applications.Count();
                var totalPages = (int)Math.Ceiling((double)totalApplications / pageSize);
                Log.Information("Applications are fetched by admin.");

                return Ok(new { items = applications, totalPages = totalPages });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Approves

        /// <summary>
        /// Approve application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost("approve-application")]
        public async Task<IActionResult> ApproveApplication(Guid applicationId)
        {
            try
            {
                var application = _dbContext.Applications.FirstOrDefault(a => a.Id == applicationId);
                if (application == null)
                {
                    return BadRequest(new { Error = "Application not found." });
                }

                application.IsVerified = true;
                _dbContext.SaveChanges();
                Log.Information("Application is approved by admin.");
                return Ok(new { Message = "Application is approved." });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Operations

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return BadRequest(new { Error = "User not found." });
                }

                var posts = _dbContext.Posts.Where(p => p.UserId == userId).ToList();
                var applications = _dbContext.Applications.Where(a => a.UserId == userId).ToList();

                FirebaseStorageService firebaseStorageService = new FirebaseStorageService();

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

                _dbContext.Posts.RemoveRange(posts);
                _dbContext.Applications.RemoveRange(applications);
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
                Log.Information("User is deleted by admin.");
                return Ok(new { Message = "User is deleted." });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Delete post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-post")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            try
            {
                var post = _dbContext.Posts.FirstOrDefault(p => p.Id == postId);
                if (post == null)
                {
                    return BadRequest(new { Error = "Post not found." });
                }

                _dbContext.Posts.Remove(post);
                _dbContext.SaveChanges();
                Log.Information("Post is deleted by admin.");
                return Ok(new { Message = "Post is deleted." });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Delete application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-application")]
        public async Task<IActionResult> DeleteApplication(Guid applicationId)
        {
            try
            {
                var application = _dbContext.Applications.FirstOrDefault(a => a.Id == applicationId);
                if (application == null)
                {
                    return BadRequest(new { Error = "Application not found." });
                }

                FirebaseStorageService firebaseStorageService = new FirebaseStorageService();

                await firebaseStorageService.DeleteApplicationFileAsync(application.Id);

                _dbContext.Applications.Remove(application);
                _dbContext.SaveChanges();
                Log.Information("Application is deleted by admin.");

                return Ok(new { Message = "Application is deleted." });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
