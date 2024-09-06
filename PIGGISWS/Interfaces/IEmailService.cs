namespace PIGGISWS.Interfaces;

public interface IEmailService
{
    Task<string> SendEmailAsync(string toEmail, string subject, string content);
}
