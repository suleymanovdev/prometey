using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prometeyapi.Core.DTOs.ApplicationDTOs.Request;
using prometeyapi.Core.DTOs.GroupDTOs.Request;
using prometeyapi.Core.DTOs.PostDTOs.Request;
using prometeyapi.Core.DTOs.PostDTOs.Response;
using prometeyapi.Core.DTOs.UserDTOs.Request;
using prometeyapi.Core.Exceptions.ApplicationExceptions;
using prometeyapi.Core.Exceptions.GroupExceptions;
using prometeyapi.Core.Exceptions.PostExceptions;
using prometeyapi.Core.Exceptions.UserExceptions;
using prometeyapi.Infrastructure.Data;
using prometeyapi.Infrastructure.Services.ApplicationServices;
using prometeyapi.Infrastructure.Services.GroupServices;
using prometeyapi.Infrastructure.Services.PostServices;
using prometeyapi.Infrastructure.Services.UserServices;
using Serilog;

namespace prometeyapi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly DBContext _dbContext;
    private readonly UserService _userService;
    private readonly PostsService _postsService;
    private readonly ApplicationService _applicationService;
    private readonly GroupService _groupService;

    public UserController(DBContext dbContext, UserService userService, PostsService postsService, ApplicationService applicationService, GroupService groupService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _postsService = postsService;
        _applicationService = applicationService;
        _groupService = groupService;
        Log.Debug("UserController Initialized.");
    }

    // GET Methods

    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(Guid id)
    {
        try
        {
            var user = await _userService.GetUser(_dbContext, id);

            Log.Information($"User {id} requested.");
            return Ok(user);
        }
        catch (GetUserRequestException ex)
        {
            Log.Error(ex.Message);
            return NotFound($"User not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing your request. {ex.Message}");
        }
    }

    /// <summary>
    /// Get all posts of a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpGet("{userId}/posts")]
    public async Task<IActionResult> GetPostsPaginationByUserId(Guid userId, int page, int pageSize = 6)
    {
        var totalPosts = await _dbContext.Posts.Where(x => x.UserId == userId).CountAsync();
        var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
        var posts = await _dbContext.Posts.AsNoTracking()
            .Where(u => u.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { posts, totalPages });
    }

    /// <summary>
    /// Get a post of a user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="postId"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpGet("{userId}/post/{postId}")]
    public async Task<ActionResult<GetPostResponseDTO>> GetPost(Guid userId, Guid postId)
    {
        try
        {
            var post = await _postsService.GetPost(_dbContext, userId, postId);

            Log.Information($"Post {postId} of {userId} requested.");
            return Ok(post);
        }
        catch (GetPostRequestException ex)
        {
            Log.Error(ex.Message);
            return NotFound($"Post not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing your request. {ex.Message}");
        }
    }

    /// <summary>
    /// Get all applications of a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpGet("{userId}/applications")]
    public async Task<IActionResult> GetApplicationsPaginationByUserId(Guid userId, int page, int pageSize = 1)
    {
        var totalApplications = await _dbContext.Applications.Where(x => x.UserId == userId).CountAsync();
        var totalPages = (int)Math.Ceiling(totalApplications / (double)pageSize);
        var applications = await _dbContext.Applications.AsNoTracking()
            .Where(u => u.UserId == userId)
            .OrderByDescending(x => x.Created)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { applications, totalPages });
    }

    /// <summary>
    /// Get an application of a user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpGet("{userId}/application/{applicationId}")]
    public async Task<IActionResult> GetApplication(Guid userId, Guid applicationId)
    {
        try
        {
            var application = await _applicationService.GetApplication(_dbContext, userId, applicationId);

            Log.Information($"Application {applicationId} requested.");
            return Ok(application);
        }
        catch (GetApplicationRequestException ex)
        {
            Log.Error(ex.Message);
            return NotFound($"Application not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing request. {ex.Message}");
        }
    }

    /// <summary>
    /// Get a group of a user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpGet("{userId}/group/{domain}")]
    public async Task<IActionResult> GetGroup(Guid userId, string domain)
    {
        try
        {
            var group = await _groupService.GetGroup(_dbContext, domain);

            Log.Information($"Group {domain} requested.");
            return Ok(group);
        }
        catch (GetGroupRequestException ex)
        {
            Log.Error(ex.Message);
            return NotFound($"Group not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing your request. {ex.Message}");
        }
    }

    /// <summary>
    /// Get all groups of a user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpGet("{userId}/groups")]
    public async Task<IActionResult> GetAllGroups(Guid userId)
    {
        try
        {
            var groups = await _groupService.GetGroups(_dbContext, userId);

            Log.Information($"Groups of {userId} requested.");
            return Ok(groups);
        }
        catch (GetGroupsRequestException ex)
        {
            Log.Error(ex.Message);
            return NotFound($"Groups not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing your request. {ex.Message}");
        }
    }

    // CREATE Methods

    /// <summary>
    /// Create a post
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpPost("{userId}/create-post")]
    public async Task<IActionResult> CreatePost(Guid userId, [FromBody] CreatePostRequestDTO request)
    {
        try
        {
            await _postsService.CreatePost(_dbContext, userId, request);
            Log.Information($"Post created by {userId}.");
            return Ok("Post created.");
        }
        catch (CreatePostRequestException ex)
        {
            Log.Error("1 " + ex.Message);
            return BadRequest($"Post not created. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error("2 " + ex.Message);
            return BadRequest($"An error occurred while processing your request. {ex.Message}");
        }
    }

    /// <summary>
    /// Create an application
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpPost("create-application")]
    [RequestSizeLimit(2024 * 1024 * 1024)]
    public async Task<IActionResult> CreateApplication([FromForm] CreateApplicationRequestDTO request)
    {
        try
        {
            await _applicationService.CreateApplication(_dbContext, request);

            Log.Information($"Application created by {request.userId}.");
            return Ok("Application created.");
        }
        catch (CreateApplicationRequestException ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"Application not created. {ex.Message}");
        }
    }

    /// <summary>
    /// Create a group
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpPost("{userId}/create-group")]
    public async Task<IActionResult> CreateGroup(Guid userId, [FromBody] CreateGroupRequestDTO request)
    {
        try
        {
            await _groupService.CreateGroup(_dbContext, userId, request);

            return Ok("Group created.");
        }
        catch (CreateGroupRequestException ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"Group not created. {ex.Message}");
        }
    }

    // UPDATE Methods

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequestDTO request)
    {
        try
        {
            await _userService.UpdateUser(_dbContext, id, request);

            Log.Information($"User {id} updated.");
            return Ok("User updated.");
        }
        catch (UpdateUserRequestException ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"User not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing your request. {ex.Message}");
        }
    }

    // DELETE Methods

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUser(_dbContext, id);

            Log.Information($"User {id} deleted.");
            return Ok( new { Message = "User deleted." } );
        }
        catch (DeleteUserRequestException ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"User not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing your request. {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a post
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="postId"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpDelete("{userId}/delete-post/{postId}")]
    public async Task<IActionResult> DeletePost(Guid userId, Guid postId)
    {
        try
        {
            await _postsService.DeletePost(_dbContext, userId, postId);

            Log.Information($"Post {postId} deleted by {userId}.");
            return Ok("Post deleted.");
        }
        catch (DeletePostRequestException ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"Post not deleted. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing your request. {ex.Message}");
        }
    }

    /// <summary>
    /// Delete an application
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    [Authorize(Roles = "USER")]
    [HttpDelete("{userId}/delete-application/{applicationId}")]
    public async Task<IActionResult> DeleteApplication(Guid userId, Guid applicationId)
    {
        try
        {
            await _applicationService.DeleteApplication(_dbContext, userId, applicationId);

            Log.Information($"Application {applicationId} deleted by {userId}.");
            return Ok();
        }
        catch (DeleteApplicationRequestException ex)
        {
            Log.Error(ex.Message);
            return NotFound($"Application not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return BadRequest($"An error occurred while processing request. {ex.Message}");
        }
    }
}