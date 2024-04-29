using GameEngine.Components;
using GameEngine.Components.UIComponents;
using GameEngine.Core.Components.Objects;
using System.Drawing;
using System.Numerics;

namespace Uno.Client.Scenes;

/// <summary>
/// The lost connection to server scene
/// </summary>
internal class LostConnectionScene : Scene
{
	public LostConnectionScene()
	{
		UIObject lostConnectionLabel = new UIObject();
		lostConnectionLabel.Text = "Lost connection to the server!";
		lostConnectionLabel.Transform.Position = new Vector3(0, 0.25f, 1);
		lostConnectionLabel.Transform.Scale = new Vector3(0.5f, 0.5f, 1);
		UIObjects.Add(lostConnectionLabel);

		UIButton returnToMainMenu = new UIButton();
		returnToMainMenu.Text = "Return to login register";
		returnToMainMenu.TextColor = Color.White;
		returnToMainMenu.Transform.Position = new Vector3(0, -0.75f, 0);
		returnToMainMenu.Transform.Scale = new Vector3(0.2f, 0.2f, 1);
		returnToMainMenu.OnFullClicked += () => new LoginRegisterScene().LoadScene();
		UIObjects.Add(returnToMainMenu);

		UICameras.Add((new UICamera(), new GameEngine.Core.Components.ViewPort(0.5f, 0.5f, 1, 1)));
	}
}