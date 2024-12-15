using FluentValidation.Results;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using prometeyapi.Core.DTOs.AuthDTOs.Request;
using prometeyapi.Core.Exceptions.AuthExceptions;
using prometeyapi.Core.Services.FirebaseStorageService;
using prometeyapi.Core.Validators;
using prometeyapi.Infrastructure.Data;
using prometeyapi.Infrastructure.Services.ImageService;
using System.Text;

namespace prometeyapi.Infrastructure.Services.AuthServices;

public class SignUpService
{
    public virtual async Task<User> UserSignUp(DBContext dbContext, SignUpRequestDTO request)
    {
        var checkUser = dbContext.Users.FirstOrDefault(u => u.Email == request.Email || u.Username == request.Username);

        if (checkUser != null)
        {
            throw new SignUpRequestException("User with this email or username already exists.");
        }

        SignUpRequestValidator signUpRequestValidator = new SignUpRequestValidator();
        ValidationResult validationResult = signUpRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            throw new SignUpRequestException($"Invalid request. {validationResult.Errors[0].ErrorMessage}.");
        }

        var imageProcessor = new ImageProcessor();
        string processedProfilePhoto = await imageProcessor.ProcessProfilePhotoAsync(request.base64ProfilePhoto);


        User user = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Username = request.Username,
            Email = request.Email,
            Password = HashPassword(request.Password),
            Category = request.Category
        };

        FirebaseStorageService firebaseStorageService = new FirebaseStorageService();

        await firebaseStorageService.UploadProfilePhotoAsync(user.Id, processedProfilePhoto);

        string? profilePhotoLink = await firebaseStorageService.GetProfilePhotoUrlAsync(user.Id);

        user.ProfilePhotoLink = profilePhotoLink;

        user.RegistrationDate = DateTime.UtcNow;

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private string HashPassword(string password)
    {
        byte[] salt = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Authentication__Salt"));
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, 256 / 8));
    }
}
