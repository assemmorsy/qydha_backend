
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Qydha.Helpers;
using Qydha.Models;

namespace Qydha.Services;

public class MailingService : IMailingService
{
    private readonly EmailSettings _mailSettings;
    public MailingService(IOptions<EmailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }
    public async Task<OperationResult<string>> SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile>? attachments = null)
    {
        var email = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Email),
            Subject = subject
        };
        email.To.Add(MailboxAddress.Parse(mailTo));
        var builder = new BodyBuilder();
        if (attachments is not null)
        {
            byte[] fileBytes;
            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }

        builder.HtmlBody = body;
        email.Body = builder.ToMessageBody();
        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

        using var smtp = new SmtpClient();
        var res = new OperationResult<string>();
        try
        {
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
            var resStr = await smtp.SendAsync(email);
            res.Data = resStr;
            res.Message = "Email sent successfully";
        }
        catch (Exception e)
        {
            res.Error = new Error() { Message = e.Message, Code = ErrorCodes.EmailSendingError };
        }

        return res;

    }

    public async Task<string> GenerateConfirmEmailBody(string otp, string requestId)
    {
        string fileName = "ConfirmEmailTemplate.html";
        string path = Path.Combine(Environment.CurrentDirectory, @"Templates\", fileName);
        var str = new StreamReader(path);
        var mailText = await str.ReadToEndAsync();
        str.Close();
        var confirmLink = $"{_mailSettings.ConfirmUrl}?Code={otp}&RequestId={requestId}";
        mailText = mailText.Replace("[confirmLink]", confirmLink);
        return mailText;
    }

}
