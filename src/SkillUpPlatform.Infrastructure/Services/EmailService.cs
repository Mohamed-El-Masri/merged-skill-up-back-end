using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillUpPlatform.Application.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SkillUpPlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            using var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]))
            {
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
            };

            var message = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
            _logger.LogInformation($"Email sent successfully to {to}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {to}");
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken)
    {
        var resetLink = $"{_configuration["FrontendUrl"]}/reset-password?token={resetToken}";
        
        var body = $@"
            <html>
            <body>
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Click the link below to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>If you did not request this, please ignore this email.</p>
                <p>This link will expire in 1 hour.</p>
            </body>
            </html>";

        return await SendEmailAsync(email, "Password Reset Request", body, true);
    }

    public async Task<bool> SendEmailVerificationAsync(string email, string verificationToken)
    {
        var verificationLink = $"{_configuration["FrontendUrl"]}/verify-email?token={verificationToken}";
        
        var body = $@"
            <html>
            <body>
                <h2>Email Verification</h2>
                <p>Please verify your email address by clicking the link below:</p>
                <a href='{verificationLink}'>Verify Email</a>
                <p>If you did not create an account, please ignore this email.</p>
            </body>
            </html>";

        return await SendEmailAsync(email, "Email Verification", body, true);
    }

    public async Task<bool> SendWelcomeEmailAsync(string email, string firstName)
    {
        var body = $@"
            <html>
            <body>
                <h2>Welcome to SkillUp Platform!</h2>
                <p>Hi {firstName},</p>
                <p>Welcome to SkillUp Platform! We're excited to have you join our learning community.</p>
                <p>Get started by exploring our learning paths and assessments.</p>
                <p>Happy Learning!</p>
            </body>
            </html>";

        return await SendEmailAsync(email, "Welcome to SkillUp Platform", body, true);
    }

    public async Task<bool> SendNotificationEmailAsync(string email, string title, string message)
    {
        var body = $@"
            <html>
            <body>
                <h2>{title}</h2>
                <p>{message}</p>
                <p>Visit your dashboard to see more details.</p>
            </body>
            </html>";

        return await SendEmailAsync(email, title, body, true);
    }
}
