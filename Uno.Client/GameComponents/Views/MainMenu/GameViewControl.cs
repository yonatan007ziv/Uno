using GameEngine.Components.UIComponents;
using GameEngine.Core.Components;
using GameEngine.Core.Components.Objects;
using System.Drawing;
using System.Numerics;
using Uno.Client.Components.Networking;
using Uno.Client.GameComponents.Elements;
using Uno.Client.Scenes;
using Uno.Core.Utilities.CommunicationProtocols;
using Uno.Core.Utilities.CommunicationProtocols.Game;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Core.Utilities.Models;

namespace Uno.Client.GameComponents.Views.MainMenu;

/// <summary>
/// The game's view control
/// </summary>
internal class GameViewControl : UIObject
{
	private const string readyText = "Press to ready";
	private const string unreadyText = "Press to unready";

	private readonly TcpClientHandler clientHandler;
	private bool controlledExit;
	private bool isReady;
	private Card lastOnStack;
	private bool didSwitchColor;
	private Color switchedToColor;

	private List<CardStackView> stackCards = new List<CardStackView>();
	private PlayersRing playersRing;
	private UIObject tableBackground;
	private UIButton quitGameButton;
	private UIButton readyButton;
	private UIButton takeCardButton;
	private UIButton callUnoButton;
	private UIButton wildRedButton;
	private UIButton wildBlueButton;
	private UIButton wildYellowButton;
	private UIButton wildGreenButton;

	public GameViewControl(TcpClientHandler lobbyConnectedClient)
	{
		clientHandler = lobbyConnectedClient;

		tableBackground = new UIObject();
		tableBackground.Meshes.Add(new GameEngine.Core.Components.MeshData("UIRect.obj", $"Table_{new Random().Next(5)}.mat"));
		Children.Add(tableBackground);

		quitGameButton = new UIButton();
		quitGameButton.Transform.Scale = new Vector3(0.075f, 0.075f, 1);
		quitGameButton.Transform.Position = new Vector3(-0.9f, -0.9f, 1);
		quitGameButton.Text = "Quit Game";
		quitGameButton.TextColor = Color.White;
		quitGameButton.OnFullClicked += ReturnToMainMenu;
		Children.Add(quitGameButton);

		readyButton = new UIButton();
		readyButton.Transform.Scale = new Vector3(0.1f, 0.1f, 1);
		readyButton.Text = readyText;
		readyButton.TextColor = Color.Green;
		readyButton.OnFullClicked += ReadyUnready;
		Children.Add(readyButton);

		// Take card from deck
		takeCardButton = new UIButton();
		takeCardButton.Transform.Scale = new Vector3(0.125f, 0.1f, 1);
		takeCardButton.Transform.Position = new Vector3(0.8f, -0.75f, 1);
		takeCardButton.Text = "Take card from deck";
		takeCardButton.TextColor = Color.Red;
		takeCardButton.OnFullClicked += TakeCardFromDeckButton;
		takeCardButton.Visible = false;
		Children.Add(takeCardButton);

		// Call uno
		callUnoButton = new UIButton();
		callUnoButton.Transform.Scale = new Vector3(0.125f, 0.1f, 1);
		callUnoButton.Transform.Position = new Vector3(0.8f, -0.55f, 1);
		callUnoButton.Text = "Call uno!";
		callUnoButton.TextColor = Color.Green;
		callUnoButton.OnFullClicked += CallUnoButton;
		callUnoButton.Visible = false;
		Children.Add(callUnoButton);

		// Color switching buttons
		wildRedButton = new UIButton();
		wildRedButton.Meshes.Add(new MeshData("UIRect.obj", "Red.mat"));
		wildRedButton.Transform.Scale = new Vector3(0.1f, 0.1f, 1);
		wildRedButton.Transform.Position = new Vector3(-0.1f, -0.2f, 1);
		wildRedButton.OnFullClicked += () => WildColor(Color.Red);
		Children.Add(wildRedButton);
		wildBlueButton = new UIButton();
		wildBlueButton.Meshes.Add(new MeshData("UIRect.obj", "Blue.mat"));
		wildBlueButton.Transform.Scale = new Vector3(0.1f, 0.1f, 1);
		wildBlueButton.Transform.Position = new Vector3(0.1f, -0.2f, 1);
		wildBlueButton.OnFullClicked += () => WildColor(Color.Blue);
		Children.Add(wildBlueButton);
		wildYellowButton = new UIButton();
		wildYellowButton.Meshes.Add(new MeshData("UIRect.obj", "Yellow.mat"));
		wildYellowButton.Transform.Scale = new Vector3(0.1f, 0.1f, 1);
		wildYellowButton.Transform.Position = new Vector3(-0.1f, -0.4f, 1);
		wildYellowButton.OnFullClicked += () => WildColor(Color.Yellow);
		Children.Add(wildYellowButton);
		wildGreenButton = new UIButton();
		wildGreenButton.Meshes.Add(new MeshData("UIRect.obj", "Green.mat"));
		wildGreenButton.Transform.Scale = new Vector3(0.1f, 0.1f, 1);
		wildGreenButton.Transform.Position = new Vector3(0.1f, -0.4f, 1);
		wildGreenButton.OnFullClicked += () => WildColor(Color.Green);
		wildRedButton.Visible = false;
		wildBlueButton.Visible = false;
		wildYellowButton.Visible = false;
		wildGreenButton.Visible = false;
		Children.Add(wildGreenButton);

		playersRing = new PlayersRing(ClickedOnCard);
		Children.Add(playersRing);

		ReadLoop();
	}

	/// <summary>
	/// The client's read loop
	/// </summary>
	private async void ReadLoop()
	{
		while (true)
		{
			string? message = await clientHandler.ReadMessage();
			if (message is null)
			{
				if (!controlledExit)
					new LostConnectionScene().LoadScene();
				return;
			}

			await InterpretMessage(message);
		}
	}

	/// <summary>
	/// Interprets a given message
	/// </summary>
	/// <param name="message"> The message to interpret </param>
	/// <returns> A task representing the state of the procedure </returns>
	private async Task InterpretMessage(string message)
	{
		await Console.Out.WriteLineAsync($"Interpreting lobby message: {message}");
		if (!GameplayMessageConstructor.GetMessageType(message, out GameMessage messageType))
			return;

		if (messageType == GameMessage.PlayerJoinedPregame)
		{
			if (GameplayMessageConstructor.DeconstructPlayerJoinedMessage(message, out PlayerGameDataModel player))
				AddEnemy(player);
		}
		else if (messageType == GameMessage.PlayerLeftPregame)
		{
			if (GameplayMessageConstructor.DeconstructPlayerLeftMessage(message, out PlayerGameDataModel player))
				RemoveEnemy(player);
		}
		else if (messageType == GameMessage.GameStarted)
			HidePregameButtons();
		else if (messageType == GameMessage.YourTurn)
			EnableTurn();
		else if (messageType == GameMessage.PlayerLost)
			OnLost(message);
		else if (messageType == GameMessage.EnableUno)
			EnableUno();
		else if (messageType == GameMessage.DisableUno)
			DisableUno();
		else if (messageType == GameMessage.PlayerWon)
			OnWon();
		else if (messageType == GameMessage.NewCardOnStack)
			UpdateLastOnStack(message);
		else if (messageType == GameMessage.TakeCard)
			TakeCard(message);
		else if (messageType == GameMessage.EnemyAddCard)
			AddCardEnemy(message);
		else if (messageType == GameMessage.EnemyRemoveCard)
			RemoveCardEnemy(message);
		else if (messageType == GameMessage.ColorSwitch)
			ColorSwitch(message);
	}

	/// <summary>
	/// Called when clicked on a card
	/// </summary>
	/// <param name="card"> Card that was clicked </param>
	private void ClickedOnCard(Card card)
	{
		if (card == Card.Wild)
		{
			wildRedButton.Visible = true;
			wildBlueButton.Visible = true;
			wildYellowButton.Visible = true;
			wildGreenButton.Visible = true;
		}
		else
			_ = clientHandler.WriteMessage(GameplayMessageConstructor.ConstructPlaceCard(card));
		DisableTurn();
	}

	/// <summary>
	/// Called when player chose a wild color
	/// </summary>
	/// <param name="color"> Chosen color </param>
	private void WildColor(Color color)
	{
		wildRedButton.Visible = false;
		wildBlueButton.Visible = false;
		wildYellowButton.Visible = false;
		wildGreenButton.Visible = false;

		_ = clientHandler.WriteMessage(GameplayMessageConstructor.ConstructColorSwitch(color));
	}

	/// <summary>
	/// Called when color was switched
	/// </summary>
	/// <param name="message"> A message containing color data </param>
	private void ColorSwitch(string message)
	{
		if (GameplayMessageConstructor.DeconstructColorSwitch(message, out Color color) && (color == Color.Red || color == Color.Blue || color == Color.Yellow || color == Color.Green))
		{
			didSwitchColor = true;
			switchedToColor = color;

			CardStackView lastCard = new CardStackView(color);
			stackCards.Add(lastCard);
			Children.Add(lastCard);
		}
	}

	/// <summary>
	/// Adds a card to an enemy player
	/// </summary>
	/// <param name="message"> Contains the enemy to add a card to </param>
	private void AddCardEnemy(string message)
	{
		if (GameplayMessageConstructor.DeconstructAddEnemyCard(message, out PlayerGameDataModel playerToAdd))
			playersRing.AddEnemyCard(playerToAdd);
	}

	/// <summary>
	/// Removes a card from an enemy player
	/// </summary>
	/// <param name="message"> Contains the enemy to remove a card from </param>
	private void RemoveCardEnemy(string message)
	{
		if (GameplayMessageConstructor.DeconstructRemoveEnemyCard(message, out PlayerGameDataModel playerToRemove))
			playersRing.RemoveEnemyCard(playerToRemove);
	}

	/// <summary>
	/// Takes a card
	/// </summary>
	/// <param name="message"> Contains the card to take </param>
	private void TakeCard(string message)
	{
		if (GameplayMessageConstructor.DeconstructPlayerTakeCard(message, out Card toTake))
			playersRing.playerCards.AddCard(toTake);
	}

	/// <summary>
	/// Update what last card is on the stack
	/// </summary>
	/// <param name="message"> Contains the card data </param>
	private void UpdateLastOnStack(string message)
	{
		if (GameplayMessageConstructor.DeconstructNewCardOnTop(message, out Card toTake))
		{
			didSwitchColor = false;
			lastOnStack = toTake;

			CardStackView lastCard = new CardStackView(toTake);
			stackCards.Add(lastCard);
			Children.Add(lastCard);
		}
	}

	/// <summary>
	/// Enable game buttons
	/// </summary>
	private void EnableTurn()
	{
		takeCardButton.Visible = true;
		playersRing.playerCards.EnableAccordingToLast(lastOnStack, didSwitchColor, switchedToColor);
	}

	/// <summary>
	/// Disable game buttons
	/// </summary>
	private void DisableTurn()
	{
		takeCardButton.Visible = false;
		playersRing.playerCards.DisableAll();
	}

	/// <summary>
	/// Enable uno callout
	/// </summary>
	private void EnableUno()
	{
		callUnoButton.Visible = true;
	}

	/// <summary>
	/// Disable uno callout
	/// </summary>
	private void DisableUno()
	{
		callUnoButton.Visible = false;
	}

	/// <summary>
	/// When the player won
	/// </summary>
	private void OnWon()
	{
		controlledExit = true;
		clientHandler.Disconnect();
		new WonScene().LoadScene();
	}

	/// <summary>
	/// When the player lost
	/// </summary>
	/// <param name="message"> Contains the winner name </param>
	private void OnLost(string message)
	{
		controlledExit = true;
		clientHandler.Disconnect();
		if (GameplayMessageConstructor.DeconstructLostGameMessage(message, out string winnerName))
			new LostScene(winnerName).LoadScene();
	}

	/// <summary>
	/// Hides the pregame buttons 
	/// </summary>
	private void HidePregameButtons()
	{
		Children.Remove(quitGameButton);
		Children.Remove(readyButton);
	}

	/// <summary>
	/// Called when interacting with the ready/unready button
	/// </summary>
	private async void ReadyUnready()
	{
		readyButton.Enabled = false;
		isReady = !isReady;
		await clientHandler.WriteMessage(GameplayMessageConstructor.ConstructPlayerReadyUnreadyMessage(isReady));
		readyButton.Text = isReady ? unreadyText : readyText;
		readyButton.TextColor = isReady ? Color.Red : Color.Green;
		readyButton.Enabled = true;
	}

	/// <summary>
	/// Takes a card from the deck
	/// </summary>
	private void TakeCardFromDeckButton()
	{
		_ = clientHandler.WriteMessage(GameplayMessageConstructor.ConstructTakeRandomCard());
		DisableTurn();
	}

	/// <summary>
	/// Calls out uno
	/// </summary>
	private void CallUnoButton()
	{
		_ = clientHandler.WriteMessage(GameplayMessageConstructor.ConstructUnoCall());
		DisableUno();
	}

	/// <summary>
	/// Add an enemy visualization
	/// </summary>
	/// <param name="enemy"> The enemy's data </param>
	private void AddEnemy(PlayerGameDataModel enemy)
	{
		playersRing.AddEnemy(enemy);
	}


	/// <summary>
	/// Removes an enemy visualization
	/// </summary>
	/// <param name="enemy"> The enemy's data </param>
	private void RemoveEnemy(PlayerGameDataModel enemy)
	{
		playersRing.RemoveEnemy(enemy);
	}

	/// <summary>
	/// Returns to the main menu
	/// </summary>
	private void ReturnToMainMenu()
	{
		controlledExit = true;
		clientHandler.Disconnect();
		new MainMenuScene().LoadScene();
	}
}