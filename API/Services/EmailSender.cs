using System.Net;
using System.Net.Mail;
using API.Interfaces;
using Microsoft.Extensions.Options;

namespace API.Services;

public class EmailSender : IEmailSender
{
    private readonly string _mail;
    private readonly string _password;
    public EmailSender(IOptions<EmailSenderSettings> config)
    {     
        _mail = config.Value.Mail;
        _password = config.Value.Password;
    }
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_mail, _password)
        };

        MailMessage _message = new MailMessage
        {
            IsBodyHtml = true,
            From = new MailAddress(_mail),
            Subject = subject,
            Body = message
        };
        _message.To.Add(email);

        return client.SendMailAsync(_message);
    }
}
