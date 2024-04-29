using Uno.Core.Utilities.CommunicationProtocols.LoginRegister;

namespace Uno.Core.Utilities.MessageConstructors;

/// <summary>
/// Constructs and deconstructs authentication requests
/// </summary>
public static class AuthenticationMessageConstructor
{
	public const char separator = '$';

	/// <summary>
	/// Constructs an authentication request
	/// </summary>
	/// <param name="username"> The username </param>
	/// <param name="authenticationToken"> The authentication token </param>
	/// <returns></returns>
	public static string ConstructAuthenticationRequest(string username, string authenticationToken)
	{
		return $"{AuthenticationProcessMessageType.AuthenticationRequest}{separator}{username}{separator}{authenticationToken}";
	}

	/// <summary>
	/// Deconstructs an authentication request
	/// </summary>
	/// <param name="data"> The raw data </param>
	/// <param name="username"> The out string username </param>
	/// <param name="authenticationToken"> The out string authenticationToken </param>
	/// <returns></returns>
	public static bool DeconstructAuthenticationRequest(string data, ref string username, out string authenticationToken)
	{
		authenticationToken = "";

		string[] parts = data.Split(separator);
		if (parts.Length < 3)
			return false;

		if (!Enum.TryParse(parts[0], out AuthenticationProcessMessageType request) || request != AuthenticationProcessMessageType.AuthenticationRequest)
			return false;

		username = parts[1];
		authenticationToken = parts[2];
		return true;
	}
}