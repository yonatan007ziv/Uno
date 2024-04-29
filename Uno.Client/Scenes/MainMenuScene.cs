using GameEngine.Components;
using GameEngine.Components.UIComponents;
using GameEngine.Core.Components;
using System.Numerics;

namespace Uno.Client.Scenes;

/// <summary>
/// The main menu scene
/// </summary>
internal class MainMenuScene : Scene
{
	private UIButton _switchToLobbySelection;
	private UIButton _switchToStats;

	public MainMenuScene()
	{
		Vector3 buttonScale = new Vector3(0.25f, 0.25f, 0);

		UICameras.Add((new UICamera(), new ViewPort(0.5f, 0.5f, 1, 1)));

		_switchToLobbySelection = new UIButton();
		_switchToLobbySelection.Text = "Lobby Selection";
		_switchToLobbySelection.TextColor = System.Drawing.Color.White;
		_switchToLobbySelection.Transform.Scale = buttonScale;
		_switchToLobbySelection.Transform.Position = new Vector3(-0.5f, 0, 0);
		_switchToLobbySelection.OnFullClicked += SwitchToLobbySelectionScene;
		UIObjects.Add(_switchToLobbySelection);

		_switchToStats = new UIButton();
		_switchToStats.Text = "My Stats";
		_switchToStats.TextColor = System.Drawing.Color.White;
		_switchToStats.Transform.Scale = buttonScale;
		_switchToStats.Transform.Position = new Vector3(0.5f, 0, 0);
		_switchToStats.OnFullClicked += SwitchToStatsScene;
		UIObjects.Add(_switchToStats);
	}

	private void SwitchToLobbySelectionScene()
	{
		new LobbySelectionScene().LoadScene();
	}

	private void SwitchToStatsScene()
	{
		new MyStatsScene().LoadScene();
	}
}