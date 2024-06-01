using System.Net;
using System.Net.Mail;
using Uno.Core.Utilities;

namespace Uno.Server.Components.Email;

/// <summary>
/// Helper class for sending emails
/// </summary>
internal static class EmailSender
{
	private static readonly SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

	static EmailSender()
	{
		smtpClient.UseDefaultCredentials = false;
		smtpClient.Port = 587;
		smtpClient.Credentials = new NetworkCredential(DevConstants.EmailCredentials.Host, DevConstants.EmailCredentials.Password);
		smtpClient.EnableSsl = true;
	}

	/// <summary>
	/// Sends an email with the given details
	/// </summary>
	/// <param name="email"> Email to send to </param>
	/// <param name="subject"> The title of the email </param>
	/// <param name="body"> The body of the email </param>
	/// <returns> True if succeeded, false otherwise </returns>
	public static bool SendEmail(string email, string subject, string body)
	{
		try
		{
			MailMessage message = new MailMessage()
			{
				From = new MailAddress(DevConstants.EmailCredentials.Host),
				Subject = subject,
				Body = body
			};
			message.To.Add(email);

			smtpClient.Send(message);
			return true;
		}
		catch { return false; }
	}
}