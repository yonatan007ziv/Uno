namespace Uno.Client.Components.Networking;

/// <summary>
/// Holds the current session critical details
/// </summary>
internal class SessionHolder
{
	public static string Username { get; set; } = "";
	public static string AuthenticationToken { get; set; } = "";
}