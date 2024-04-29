using System.Text;

namespace Uno.Server.Components;

/// <summary>
/// Helper class for generating a random authentication token
/// </summary>
internal class AuthenticationCodeGenerator
{
	private static readonly Random random = new Random();

	/// <summary>
	/// Generates a random authentication token
	/// </summary>
	/// <returns> The authentication token </returns>
	public static string GenerateAuthenticationCode()
	{
		const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		StringBuilder res = new StringBuilder();
		for (int i = 0; i < 15; i++)
			res.Append(valid[random.Next(valid.Length)]);
		return res.ToString();
	}
}