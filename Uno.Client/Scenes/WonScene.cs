﻿using GameEngine.Components;
using GameEngine.Components.UIComponents;
using GameEngine.Core.Components.Objects;
using System.Drawing;
using System.Numerics;

namespace Uno.Client.Scenes;

/// <summary>
/// Won the game scene
/// </summary>
internal class WonScene : Scene
{
	public WonScene()
	{
		UIObject lostLabel = new UIObject();
		lostLabel.Text = $"You Won!";
		lostLabel.TextColor = Color.Green;
		lostLabel.Transform.Position = new Vector3(0, 0.25f, 1);
		lostLabel.Transform.Scale = new Vector3(0.5f, 0.5f, 1);
		UIObjects.Add(lostLabel);

		UIButton returnToLobbySelectionButton = new UIButton();
		returnToLobbySelectionButton.Text = "Return to lobby selection";
		returnToLobbySelectionButton.TextColor = Color.White;
		returnToLobbySelectionButton.Transform.Position = new Vector3(0, -0.75f, 0);
		returnToLobbySelectionButton.Transform.Scale = new Vector3(0.2f, 0.2f, 1);
		returnToLobbySelectionButton.OnFullClicked += () => new LobbySelectionScene().LoadScene();
		UIObjects.Add(returnToLobbySelectionButton);

		UICameras.Add((new UICamera(), new GameEngine.Core.Components.ViewPort(0.5f, 0.5f, 1, 1)));
	}
}