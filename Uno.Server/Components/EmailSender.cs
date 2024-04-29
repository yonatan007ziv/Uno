using System.Net;
using System.Net.Mail;

namespace Uno.Server.Components;

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
		smtpClient.Credentials = new NetworkCredential(EmailConstants.Host, EmailConstants.Password);
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
			smtpClient.Send(EmailConstants.Host, email, subject, body);
			return true;
		}
		catch { return false; }
	}
}