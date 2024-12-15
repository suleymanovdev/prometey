using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using prometeyapi.Core.DTOs.AuthDTOs.Request;
using prometeyapi.Core.DTOs.AuthDTOs.Response;
using prometeyapi.Core.Enums;
using prometeyapi.Core.Exceptions.AuthExceptions;
using prometeyapi.Infrastructure.Data;
using prometeyapi.Infrastructure.Services.AuthServices;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace prometeyapi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DBContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly MailService _mailService;
    private readonly SignUpService _signUpService;
    private readonly LogInService _logInService;

    public AuthController(DBContext dbContext, IMemoryCache memoryCache, MailService mailService, SignUpService signUpService, LogInService logInService)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _mailService = mailService;
        _signUpService = signUpService;
        _logInService = logInService;
        Log.Debug("AuthController Initialized.");
    }

    /// <summary>
    /// Sign up
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequestDTO request)
    {
        try
        {
            User user = await _signUpService.UserSignUp(_dbContext, request);

            await _mailService.SendEmailVerification(_memoryCache, user.Email, user.Id);

            Log.Information("User signup request received.");
            Log.Information($"Verification request sent to {request.Email}.");
            return Ok(new SignUpResponseDTO { Message = "Registration successful. Please check your email for verification." });
        }
        catch (SignUpRequestException ex)
        {
            Log.Error(ex.Message);
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Log in
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LogInRequestDTO request)
    {
        if (request.Email == Environment.GetEnvironmentVariable("ADMIN__Login") && request.Password == Environment.GetEnvironmentVariable("ADMIN__Password"))
        {
            string adminJwtToken = GenerateJwtToken(Guid.Empty, Role.ADMIN, Status.PREMIUM);
            _memoryCache.Set(Guid.Empty, adminJwtToken, TimeSpan.FromMinutes(15));

            Log.Information("Admin login request received.");
            return Ok(new LogInResponseDTO { Token = adminJwtToken, UserId = "ADMIN" });
        }

        try
        {
            var user = await _logInService.UserLogIn(_dbContext, request);
            string jwtToken = GenerateJwtToken(user.Id, user.Role, user.Status);
            _memoryCache.Set(user.Id, jwtToken, TimeSpan.FromMinutes(15));

            Log.Information($"User login request received for {user.Id}.");
            return Ok(new LogInResponseDTO { Token = jwtToken, UserId = user.Id.ToString() });
        }
        catch (LogInRequestException ex)
        {
            Log.Error($"LogInRequestException: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Verify email
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("verify-email/{userId}")]
    public async Task<IActionResult> VerifyEmail([FromRoute] Guid userId, [FromQuery] string token)
    {
        try
        {
            bool IsVerified = _mailService.EmailVerification(_dbContext, _memoryCache, userId, token).Result;

            if (IsVerified)
            {
                Log.Information($"Email verification successful for user {userId}.");
                return RedirectPermanent("http://localhost:3000/verified");
            }
            else
            {
                Log.Error($"Email verification failed for user {userId}.");
                return BadRequest("Email verification failed.");
            }
        }
        catch (MailVerificationException ex)
        {
            Log.Error($"Email verification error.");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Generate JWT token
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userRole"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    private string GenerateJwtToken(Guid userId, Role userRole, Status status)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT__Key")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId.ToString()),
            new Claim(ClaimTypes.Role, userRole.ToString()),
            new Claim(ClaimTypes.Role, status.ToString())
        };

        var token = new JwtSecurityToken(
            Environment.GetEnvironmentVariable("JWT__Issuer"),
            Environment.GetEnvironmentVariable("JWT__Audience"),
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
