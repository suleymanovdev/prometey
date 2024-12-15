using System.Text;
using prometeyapi.Core.Models;
using FluentValidation.Results;
using prometeyapi.Core.Validators;
using Microsoft.EntityFrameworkCore;
using prometeyapi.Infrastructure.Data;
using prometeyapi.Core.DTOs.AuthDTOs.Request;
using prometeyapi.Core.Exceptions.AuthExceptions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace prometeyapi.Infrastructure.Services.AuthServices;

public class LogInService
{
    public virtual async Task<User> UserLogIn(DBContext dbContext, LogInRequestDTO request)
    {
        LogInRequestValidator logInRequestValidator = new LogInRequestValidator();
        ValidationResult validationResult = logInRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            throw new LogInRequestException("Invalid request.");
        }

        User user = await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == request.Email && u.Password == HashPassword(request.Password));

        if (user == null)
        {
            throw new LogInRequestException("Invalid credentials.");
        }

        if (!user.IsVerified)
        {
            throw new LogInRequestException("Email not verified. Register again.");
        }

        return user;
    }

    public string HashPassword(string password)
    {
        byte[] salt = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Authentication__Salt"));
        string res = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, 256 / 8));
        return res;
    }
}