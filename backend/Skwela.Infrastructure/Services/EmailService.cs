using Skwela.Application.Interfaces;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Skwela.Infrastructure.Services;

public class EmailService : IEmailService
{
  private readonly IConfiguration _config;

  public EmailService(IConfiguration config)
  {
    _config = config;
  }

  public async Task SendOtpEmailAsync(string toEmail, string otpCode)
  {
    var smtpServer = _config["Email:SmtpServer"];
    var smtpPort = int.Parse(_config["Email:SmtpPort"]!);
    var senderEmail = _config["Email:SenderEmail"];
    var senderPassword = _config["Email:SenderPassword"];

    using var client = new SmtpClient(smtpServer, smtpPort)
    {
      Credentials = new NetworkCredential(senderEmail, senderPassword),
      EnableSsl = true
    };

    var mailMessage = new MailMessage
    {
      From = new MailAddress(senderEmail!, "Skwela Security"),
      Subject = "Your Skwela Verification Code",
      Body = $"<h2>Welcome!</h2><p>Your 6-digit verification code is: <strong>{otpCode}</strong></p><p>This code will expire in 5 minutes.</p>",
      IsBodyHtml = true
    };

    mailMessage.To.Add(toEmail);

    await client.SendMailAsync(mailMessage);
  }
}