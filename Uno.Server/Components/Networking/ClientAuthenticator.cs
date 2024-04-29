


namespace Uno.Server.Components.Networking;

/// <summary>
/// Used to authenticate clients
/// </summary>
internal class ClientAuthenticator
{
	// Username , AuthenticationKey
	private static readonly List<(string username, string authenticationToken)> authenticatedUsers = new List<(string, string)>();

	/// <summary>
	/// Checks if the given username authentication token pair is authenticated
	/// </summary>
	/// <param name="username"> The username to check authentication for </param>
	/// <param name="authenticationToken"> The authentication token </param>
	/// <returns> True if the username is authenticated, false otherwise </returns>
	public static bool CheckAuthenticationToken(string username, string authenticationToken)
	{
		for (int i = 0; i < authenticatedUsers.Count; i++)
			if (authenticatedUsers[i].username == username && authenticatedUsers[i].authenticationToken == authenticationToken)
				return true;
		return false;
	}

	/// <summary>
	/// Generates an authentication token for a username, and stores it as authenticated
	/// </summary>
	/// <param name="username"> The authenticated username to generate a token for </param>
	/// <returns> The authentication token </returns>
	public static string GenerateAuthenticationToken(string username)
	{
		string code = AuthenticationCodeGenerator.GenerateAuthenticationCode();
		if (authenticatedUsers.Any(t => t.authenticationToken == code))
			return GenerateAuthenticationToken(username);

		authenticatedUsers.Add((username, code));
		return code;
	}
}