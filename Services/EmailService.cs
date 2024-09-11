
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using CEBlog.ViewModels;

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
			var email = new MimeMessage();
			email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
			email.To.Add(MailboxAddress.Parse(_mailSettings.Mail));
			email.Subject = "New Entry: Contact Form";

			var builder = new BodyBuilder();
			builder.HtmlBody = $"<b>{firstName} {lastName}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

			email.Body = builder.ToMessageBody();

			using var smtp = new SmtpClient();
			smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

			await smtp.SendAsync(email);

			smtp.Disconnect(true);
		}

        public async Task SendSubscribeEmailAsync(string emailFrom)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(_mailSettings.Mail));
            email.Subject = "New Subscriber";

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<b>Email: </b>{emailFrom}";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
		{
			var email = new MimeMessage();
			email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
			email.To.Add(MailboxAddress.Parse(emailTo));
			email.Subject= subject;

			var builder = new BodyBuilder()
			{
				HtmlBody = htmlMessage
			};

			email.Body = builder.ToMessageBody();

			using var smtp = new SmtpClient();
			smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

			await smtp.SendAsync(email);

			smtp.Disconnect(true);
		}
	}
}
