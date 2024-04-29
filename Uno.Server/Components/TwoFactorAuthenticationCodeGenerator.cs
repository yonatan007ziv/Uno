using System.Text;

namespace Uno.Server.Components;

internal static class TwoFactorAuthenticationCodeGenerator
{
	private static readonly Random random = new Random();

	/// <summary>
	/// Generates a random 2fa code
	/// </summary>
	/// <returns> The random 2fa code </returns>
	public static string Generate2FACode()
	{
		const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		StringBuilder res = new StringBuilder();
		for (int i = 0; i < 5; i++)
			res.Append(valid[random.Next(valid.Length)]);
		return res.ToString();
	}
}