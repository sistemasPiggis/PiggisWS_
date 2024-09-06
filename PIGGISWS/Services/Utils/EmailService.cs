using System.Net.Mail;
using System.Net;
using PIGGISWS.Interfaces;
using Microsoft.Extensions.Options;

namespace PIGGISWS.Services.Utils;

    public class EmailService :IEmailService
    {
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    public async Task<string> SendEmailAsync(string toEmails, string subject, string content)
        {
        
            var smtpClient = new SmtpClient("mail.piggis.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("asistemas1@piggis.com"),
                Subject = subject,
                Body = $"<html><body style='font-family: Arial, sans-serif;'><h2 style='color: #4A90E2;'>{subject}</h2><p style='color: #333;'>{content.Replace("\n", "<br/>")}</p></body></html>",
                IsBodyHtml = true,
            };

            // Separar los correos
            var emailAddresses = toEmails.Split(';');
            foreach (var email in emailAddresses)
            {
                if (!string.IsNullOrWhiteSpace(email)) 
                {
                    mailMessage.To.Add(email.Trim()); // Eliminar espacios en blanco extra
                }
            }

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                return " Correo Enviado Correctamente";
            }
            catch (Exception ex)
            {
                return " Failed to send email. " + ex.Message;
            }
        }
    }
