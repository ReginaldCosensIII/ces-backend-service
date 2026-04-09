using CES.BackendService.Contracts;
using CES.BackendService.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CES.BackendService.Services;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;

    public SmtpEmailService(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendRegistrationEmailAsync(ForumRegistrationRequest request)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("CES Website", _options.FromAddress));
        message.To.Add(new MailboxAddress("Mr. Robertson", _options.ToAddress));
        message.Subject = $"New CEO AI Forum Registration: {request.FirstName} {request.LastName}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <p>A new registration has been submitted for the CEO AI Forum.</p>
                <p><strong>Details:</strong><br>
                - First Name: {request.FirstName}<br>
                - Last Name: {request.LastName}<br>
                - Title: {request.Title}<br>
                - Company: {request.Company}<br>
                - Email: {request.Email}<br>
                - Phone: {request.Phone}</p>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.Auto);
        
        if (!string.IsNullOrEmpty(_options.Username))
        {
            await client.AuthenticateAsync(_options.Username, _options.Password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
