using GameEngine.Components;
using Uno.Client.Components.Networking;
using Uno.Client.GameComponents.Views.MainMenu;

namespace Uno.Client.Scenes;

/// <summary>
/// The actual game scene
/// </summary>
internal class GameScene : Scene
{
	public GameScene(TcpClientHandler lobbyConnectedClient)
	{
		UICameras.Add((new UICamera(), new GameEngine.Core.Components.ViewPort(0.5f, 0.5f, 1, 1)));
		UIObjects.Add(new GameViewControl(lobbyConnectedClient));
	}
}