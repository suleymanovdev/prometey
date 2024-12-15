using System.Net;
using System.Net.Mail;
using prometeyapi.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;
using prometeyapi.Core.Exceptions.AuthExceptions;

namespace prometeyapi.Infrastructure.Services.AuthServices;

public class MailService
{
    public virtual async Task SendEmailVerification(IMemoryCache memoryCache, string email, Guid userId)
    {
        string verificationToken = Guid.NewGuid().ToString();
        memoryCache.Set(userId, verificationToken, TimeSpan.FromMinutes(3));

        string verificationLink = $"http://localhost:5205/api/auth/verify-email/{userId}?token={verificationToken}";

        using (var client = new SmtpClient(Environment.GetEnvironmentVariable("MailSettings__SmtpServer"), Convert.ToInt32(Environment.GetEnvironmentVariable("MailSettings__SmtpPort"))))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("MailSettings__Email"), Environment.GetEnvironmentVariable("MailSettings__Password"));

            var message = new MailMessage
            {
                From = new MailAddress(Environment.GetEnvironmentVariable("MailSettings__Email"), "PROMETEY SUPPORT"),
                Subject = "Email Verification",
                Body = $@"
                        <html>
                        <body>
                            <p>Please confirm that you want use this as your PROMETEY account email address. To confirm your email address, click the button below:</p>
                            <a href='{verificationLink}' style='background-color: #4CAF50; border: none; color: white; padding: 15px 32px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer;'>Verify Email</a>
                        </body>
                        </html>",
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(email));

            await client.SendMailAsync(message);
        }
    }

    public virtual async Task<bool> EmailVerification(DBContext dbContext, IMemoryCache memoryCache, Guid userId, string token)
    {
        if (!memoryCache.TryGetValue(userId, out string storedToken) || storedToken != token)
        {
            throw new MailVerificationException("Invalid verification request.");
        }

        var user = await dbContext.Users.FindAsync(userId);

        if (user == null)
        {
            throw new MailVerificationException("User not found.");
        }

        if (user.IsVerified)
        {
            throw new MailVerificationException("Email already verified.");
        }

        user.IsVerified = true;

        await dbContext.SaveChangesAsync();

        memoryCache.Remove(userId);

        return true;
    }
}