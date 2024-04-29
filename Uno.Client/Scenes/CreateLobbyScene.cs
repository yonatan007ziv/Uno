using GameEngine.Components;
using GameEngine.Components.UIComponents;
using GameEngine.Core.Components;
using System.Net;
using Uno.Client.Components;
using Uno.Client.Components.Networking;
using Uno.Client.GameComponents;
using Uno.Core.Utilities.CommunicationProtocols.Lobby;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Core.Utilities.Models;
using Uno.Core.Utilities.Networking;

namespace Uno.Client.Scenes;

/// <summary>
/// The create lobby scene
/// </summary>
internal class CreateLobbyScene : Scene
{
	private UITextBox lobbyNameTextBox;
	private UIButton createLobbyButton;

	public CreateLobbyScene()
	{
		UICameras.Add((new UICamera(), new ViewPort(0.5f, 0.5f, 1, 1)));

		UILabel lobbyNameLabel = new UILabel();
		lobbyNameLabel.Text = "Lobby Name:";
		lobbyNameLabel.TextColor = System.Drawing.Color.White;
		lobbyNameLabel.Transform.Scale /= 5;
		lobbyNameLabel.Transform.Position = new System.Numerics.Vector3(-0.25f, 0.75f, 0);
		UIObjects.Add(lobbyNameLabel);

		lobbyNameTextBox = new UITextBox();
		lobbyNameTextBox.TextColor = System.Drawing.Color.White;
		lobbyNameTextBox.Transform.Scale /= 5;
		lobbyNameTextBox.Transform.Position = new System.Numerics.Vector3(0.25f, 0.75f, 0);
		UIObjects.Add(lobbyNameTextBox);

		createLobbyButton = new UIButton();
		createLobbyButton.Text = "Create Lobby";
		createLobbyButton.TextColor = System.Drawing.Color.White;
		createLobbyButton.Transform.Scale /= 5;
		createLobbyButton.Transform.Position = new System.Numerics.Vector3(0, -0.2f, 0);
		createLobbyButton.OnFullClicked += OnCreateButton;
		UIObjects.Add(createLobbyButton);

		// Back button
		UIButton backButton = new UIButton();
		backButton.Text = "Back";
		backButton.TextColor = System.Drawing.Color.White;
		backButton.Transform.Scale /= 5;
		backButton.Transform.Position = new System.Numerics.Vector3(-0.75f, -0.75f, 0);
		backButton.OnFullClicked += new MainMenuScene().LoadScene;
		UIObjects.Add(backButton);
	}

	/// <summary>
	/// Called when pressing the create lobby button
	/// </summary>
	private async void OnCreateButton()
	{
		createLobbyButton.Enabled = false;
		await CreateLobbyProcedure();
		createLobbyButton.Enabled = true;
	}

	/// <summary>
	/// Create lobby procedure
	/// </summary>
	/// <returns> A task representing the state of the procedure </returns>
	private async Task CreateLobbyProcedure()
	{
		if (!Factories.ClientFactory.Create(out TcpClientHandler client)
			|| !await client.Connect(IPAddress.Parse(ServerAddresses.GameplayServerAddress), ServerAddresses.GameplayServerPort))
			return;

		if (!await client.WriteMessage(AuthenticationMessageConstructor.ConstructAuthenticationRequest(SessionHolder.Username, SessionHolder.AuthenticationToken)))
			return;

		LobbyModel lobbyData = new LobbyModel() { HostName = SessionHolder.Username, Name = lobbyNameTextBox.Text };
		if (!await client.WriteMessage(LobbyMessageConstructor.ConstructLobbyCreateRequest(lobbyData)))
			return;

		string? responseStr = await client.ReadMessage();
		if (responseStr is null)
			return;

		client.Disconnect();


		if (LobbyMessageConstructor.DeconstructLobbyCreateResponse(responseStr, out LobbyCreateJoinResponse response, out LobbyModel createdLobbyModel))
			if (response == LobbyCreateJoinResponse.Success)
				LobbyJoiner.JoinLobby(createdLobbyModel.Id);
	}
}