using GameEngine.Components;
using GameEngine.Components.UIComponents;
using System.Drawing;
using System.Net;
using Uno.Client.Components;
using Uno.Client.Components.Networking;
using Uno.Core.Utilities;
using Uno.Core.Utilities.MessageConstructors;

namespace Uno.Client.Scenes;

/// <summary>
/// The stats scene
/// </summary>
internal class MyStatsScene : Scene
{
	private readonly UIButton backButton;
	private readonly UILabel wonCounterLabel;
	private readonly UILabel lostCounterLabel;

	public MyStatsScene()
	{
		UICameras.Add((new UICamera(), new GameEngine.Core.Components.ViewPort(0.5f, 0.5f, 1, 1)));

		wonCounterLabel = new UILabel();
		wonCounterLabel.Transform.Scale = new System.Numerics.Vector3(0.5f, 0.3f, 1);
		wonCounterLabel.Transform.Position = new System.Numerics.Vector3(0, 0.3f, 0);
		wonCounterLabel.TextColor = Color.Green;
		UIObjects.Add(wonCounterLabel);

		lostCounterLabel = new UILabel();
		lostCounterLabel.Transform.Scale = new System.Numerics.Vector3(0.5f, 0.3f, 1);
		lostCounterLabel.Transform.Position = new System.Numerics.Vector3(0, -0.3f, 0);
		lostCounterLabel.TextColor = Color.Red;
		UIObjects.Add(lostCounterLabel);

		// Back button
		backButton = new UIButton();
		backButton.Text = "Back to Main Menu";
		backButton.TextColor = Color.White;
		backButton.Transform.Scale /= 5;
		backButton.Transform.Position = new System.Numerics.Vector3(-0.75f, -0.75f, 0);
		backButton.OnFullClicked += new MainMenuScene().LoadScene;
		UIObjects.Add(backButton);

		GetStats();
	}

	/// <summary>
	/// Gets the stats of the current player
	/// </summary>
	private async void GetStats()
	{
		if (!Factories.ClientFactory.Create(out TcpClientHandler clientHandler))
			return;

		if (!await clientHandler.Connect(IPAddress.Parse(DevConstants.GameplayServerAddress), DevConstants.GameplayServerPort))
			return;

		if (!await clientHandler.WriteMessage(AuthenticationMessageConstructor.ConstructAuthenticationRequest(SessionHolder.Username, SessionHolder.AuthenticationToken)))
			return;

		if (!await clientHandler.WriteMessage(LobbyMessageConstructor.ConstructStatsRequest(SessionHolder.Username)))
			return;

		// Read login response from server
		string? responseStr = await clientHandler.ReadMessage();
		if (responseStr is null)
			return;

		if (LobbyMessageConstructor.DeconstructStatsResponse(responseStr, out int wonTimes, out int lostTimes))
		{
			wonCounterLabel.Text = $"You won {wonTimes} times!";
			lostCounterLabel.Text = $"You lost {lostTimes} times!";
		}
		else
		{
			wonCounterLabel.Text = "Failed to get stats!";
		}
	}
}