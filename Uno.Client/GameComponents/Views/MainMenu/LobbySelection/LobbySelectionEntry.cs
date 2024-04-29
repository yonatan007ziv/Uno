using GameEngine.Components.UIComponents;
using System.Drawing;
using System.Numerics;
using Uno.Core.Utilities;
using Uno.Core.Utilities.Models;

namespace Uno.Client.GameComponents.Views.MainMenu.LobbySelection;

/// <summary>
/// An entry for the lobbies list
/// </summary>
internal class LobbySelectionEntry : UIButton
{
	public int LobbyId { get; }

	public LobbySelectionEntry(LobbyModel lobby)
	{
		LobbyId = lobby.Id;

		// Lobby name label
		UILabel lobbyNameLabel = new UILabel();
		lobbyNameLabel.Text = lobby.Name;
		lobbyNameLabel.TextColor = Color.White;
		lobbyNameLabel.Transform.Scale = new Vector3(0.2f, 1, 1);
		lobbyNameLabel.Transform.Position = new Vector3(-0.5f, 0, 0);
		Children.Add(lobbyNameLabel);

		// Host name label
		UILabel hostNameLabel = new UILabel();
		hostNameLabel.Text = lobby.HostName;
		hostNameLabel.TextColor = Color.White;
		hostNameLabel.Transform.Scale = new Vector3(0.2f, 1, 1);
		hostNameLabel.Transform.Position = new Vector3(0, 0, 0);
		Children.Add(hostNameLabel);

		// Player count label
		UILabel playerCountLabel = new UILabel();
		playerCountLabel.Text = $"{lobby.CurrentPlayerCount}/{GameConstants.MaxPlayerCount}";
		playerCountLabel.TextColor = Color.White;
		playerCountLabel.Transform.Scale = new Vector3(0.2f, 1, 1);
		playerCountLabel.Transform.Position = new Vector3(0.5f, 0, 0);
		Children.Add(playerCountLabel);
	}
}