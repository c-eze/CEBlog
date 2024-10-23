
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using CEBlog.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CEBlog.Services
{
    public class EmailService : IBlogEmailSender
	{
		private readonly MailSettings _mailSettings;

		public EmailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}

		public async Task SendContactEmailAsync(string emailFrom, string firstName, string lastName, string htmlMessage)
		{
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            var email = new MimeMessage();
			email.Sender = MailboxAddress.Parse(_mailSettings.Email);
			email.To.Add(MailboxAddress.Parse(_mailSettings.Email));
			email.Subject = "New Entry: Contact Form";

			var builder = new BodyBuilder();
			builder.HtmlBody = $"<b>{firstName} {lastName}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

			email.Body = builder.ToMessageBody();

			using var smtpClient = new SmtpClient();

            try
            {
                var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
                var port = _mailSettings.MailPort != 0 ? _mailSettings.MailPort : int.Parse(Environment.GetEnvironmentVariable("MailPort")!);
                var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");

                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, password);

                await smtpClient.SendAsync(email);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
		}

        public async Task SendSubscribeEmailAsync(string emailFrom)
        {
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Email);
            email.To.Add(MailboxAddress.Parse(_mailSettings.Email));
            email.Subject = "New Subscriber";

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<b>Email: </b>{emailFrom}";

            email.Body = builder.ToMessageBody();

            using var smtpClient = new SmtpClient();

            try
            {
                var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
                var port = _mailSettings.MailPort != 0 ? _mailSettings.MailPort : int.Parse(Environment.GetEnvironmentVariable("MailPort")!);
                var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");

                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, password);

                await smtpClient.SendAsync(email);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            var email = new MimeMessage();
			email.Sender = MailboxAddress.Parse(_mailSettings.Email);
			email.To.Add(MailboxAddress.Parse(emailTo));
			email.Subject= subject;

			var builder = new BodyBuilder()
			{
				HtmlBody = htmlMessage
			};

			email.Body = builder.ToMessageBody();

			using var smtpClient = new SmtpClient();

            try
            {
                var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
                var port = _mailSettings.MailPort != 0 ? _mailSettings.MailPort : int.Parse(Environment.GetEnvironmentVariable("MailPort")!);
                var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");

                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, password);

                await smtpClient.SendAsync(email);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
	}
}
