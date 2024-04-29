
using System.Net;
using Uno.Client.Components;
using Uno.Client.Components.Networking;
using Uno.Client.Scenes;
using Uno.Core.Utilities.CommunicationProtocols.Lobby;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Core.Utilities.Networking;

namespace Uno.Client.GameComponents;

/// <summary>
/// Acts as a lobby joiner
/// </summary>
internal class LobbyJoiner
{
	private static TcpClientHandler clientHandler = null!;

	/// <summary>
	/// Join a game by id
	/// </summary>
	/// <param name="id"> The lobby id to join </param>
	public static async void JoinLobby(int id)
	{
		if (await InitializeLobbyConnection(id))
			new GameScene(clientHandler).LoadScene();
	}


	/// <summary>
	/// Initializes the lobby game connection
	/// </summary>
	/// <param name="lobbyId"></param>
	/// <returns> True if the connection succeeded, false otherwise </returns>
	private static async Task<bool> InitializeLobbyConnection(int lobbyId)
	{
		if (!Factories.ClientFactory.Create(out clientHandler)
			|| !await clientHandler.Connect(IPAddress.Parse(ServerAddresses.GameplayServerAddress), ServerAddresses.GameplayServerPort))
			OnCritical();

		if (!await clientHandler.WriteMessage(AuthenticationMessageConstructor.ConstructAuthenticationRequest(SessionHolder.Username, SessionHolder.AuthenticationToken)))
			OnCritical();

		if (!await clientHandler.WriteMessage(LobbyMessageConstructor.ConstructLobbyJoinRequest(lobbyId)))
			OnCritical();

		string? responseStr = await clientHandler.ReadMessage();
		if (responseStr is null)
		{
			OnCritical();
			return false;
		}

		if (LobbyMessageConstructor.DeconstructLobbyJoinResponse(responseStr, out LobbyCreateJoinResponse response))
			if (response == LobbyCreateJoinResponse.Success)
				return true;

		OnCritical();
		return false;
	}

	/// <summary>
	/// Occurs when a critical problem faced
	/// </summary>
	private static void OnCritical()
	{
		clientHandler.Disconnect();
		new LostConnectionScene().LoadScene(); // Return to main menu
	}
}