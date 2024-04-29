using GameEngine.Core.Components.Objects;
using System.Net;
using Uno.Client.Components;
using Uno.Client.Components.Networking;
using Uno.Core.Utilities.CommunicationProtocols.Lobby;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Core.Utilities.Models;
using Uno.Core.Utilities.Networking;

namespace Uno.Client.GameComponents.Views.MainMenu.LobbySelection;

/// <summary>
/// The lobbies selection list view control
/// </summary>
internal class LobbiesWindowViewControl : UIObject
{
	private TcpClientHandler clientHandler;

	public LobbiesWindowViewControl()
	{
		Meshes.Add(new GameEngine.Core.Components.MeshData("UIRect.obj", "Gray.mat"));

		OnUnloaded += Close;
		clientHandler = null!;
		InitializeLobbyListener();
	}

	/// <summary>
	/// Initialized the lobby listener client
	/// </summary>
	private async void InitializeLobbyListener()
	{
		if (!Factories.ClientFactory.Create(out clientHandler)
			|| !await clientHandler.Connect(IPAddress.Parse(ServerAddresses.GameplayServerAddress), ServerAddresses.GameplayServerPort))
			return;

		// Authenticate user
		if (!await clientHandler.WriteMessage(AuthenticationMessageConstructor.ConstructAuthenticationRequest(SessionHolder.Username, SessionHolder.AuthenticationToken)))
			return;

		if (!await clientHandler.WriteMessage(LobbyMessageConstructor.ConstructNotifyInLobbySelection()))
			return;

		ListenForLobbyUpdates();
	}

	/// <summary>
	/// Listen for lobby updates
	/// </summary>
	private async void ListenForLobbyUpdates()
	{
		while (true)
		{
			string? responseStr = await clientHandler.ReadMessage();
			if (responseStr is null)
				return;

			InterpretLobbyMessage(responseStr);
		}
	}

	/// <summary>
	/// Interpret the lobbies update message
	/// </summary>
	/// <param name="message"> The received message containing the lobbies update </param>
	private void InterpretLobbyMessage(string message)
	{
		if (!LobbyMessageConstructor.GetMessageType(message, out LobbyMessage messageType))
			return;

		if (messageType == LobbyMessage.LobbyList)
			PopulateLobbies(message);
	}

	/// <summary>
	/// Populates lobbies based on a message containing the lobbies' data
	/// </summary>
	/// <param name="message"> The message containing the data </param>
	private void PopulateLobbies(string message)
	{
		Children.Clear();

		if (LobbyMessageConstructor.DeconstructLobbiesListMessage(message, out List<LobbyModel> lobbies))
		{
			float yScale = 1f / lobbies.Count;
			float yOffset = 1;

			for (int i = 0; i < lobbies.Count; i++)
			{
				LobbySelectionEntry lobbyEntry = new LobbySelectionEntry(lobbies[i]);
				lobbyEntry.TextColor = System.Drawing.Color.White;
				lobbyEntry.Transform.Position = new System.Numerics.Vector3(0, yOffset - yScale, 1);
				lobbyEntry.Transform.Scale = new System.Numerics.Vector3(1, yScale, 1);
				lobbyEntry.OnFullClicked += () => LobbyJoiner.JoinLobby(lobbyEntry.LobbyId);

				Children.Add(lobbyEntry);

				yOffset -= yScale * 2;
			}
		}
	}

	/// <summary>
	/// Closes the current connection
	/// </summary>
	public void Close()
	{
		clientHandler.Disconnect();
	}
}