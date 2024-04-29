using System.Drawing;
using Uno.Core.Utilities;
using Uno.Core.Utilities.CommunicationProtocols;
using Uno.Core.Utilities.CommunicationProtocols.Game;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Core.Utilities.Models;
using Uno.Server.Components.Database;
using Uno.Server.Components.Networking.ClientHandlers;

namespace Uno.Server.Components.Lobby;

/// <summary>
/// Handles all lobby requests and game logic
/// </summary>
internal class LobbyHandler
{
	private int currentPlayerId;
	private readonly Dictionary<AuthenticatedSessionClientHandler, PlayerGameDataModel> players = new Dictionary<AuthenticatedSessionClientHandler, PlayerGameDataModel>();

	private AuthenticatedSessionClientHandler? lastInThreatOfUno;
	public LobbyModel lobbyModel;

	public LobbyHandler(LobbyModel lobbyModel)
	{
		this.lobbyModel = lobbyModel;
	}

	/// <summary>
	/// Returns whether the lobby is full or not
	/// </summary>
	/// <returns></returns>
	public bool LobbyFull()
		=> players.Count >= GameConstants.MaxPlayerCount;

	/// <summary>
	/// Interprets the message received by a lobby player
	/// </summary>
	/// <param name="sender"> The sender </param>
	/// <param name="message"> The received message </param>
	/// <returns> A task representing the message's processing process </returns>
	public async Task InterpretMessage(AuthenticatedSessionClientHandler sender, string message)
	{
		await Console.Out.WriteLineAsync($"Got lobby message from \'{sender.Username}\': {message}");
		if (!GameplayMessageConstructor.GetMessageType(message, out GameMessage messageType))
			return;

		if (messageType == GameMessage.PlayerReadyUnready)
			PlayerReadyUnready(sender, message);
		// Player actions
		else if (messageType == GameMessage.TakeRandomCard)
		{
			PlayerTakeCard(sender);
			AdvancePlayerTurn();
		}
		else if (messageType == GameMessage.PlaceCard)
			PlayerPlaceCard(sender, message);
		else if (messageType == GameMessage.ColorSwitch)
			SwitchColor(sender, message);
		else if (messageType == GameMessage.CallUno)
		{
			if (sender != lastInThreatOfUno && lastInThreatOfUno is not null)
				for (int i = 0; i < 4; i++) // Take 4 cards if someone said Uno before
					PlayerTakeCard(lastInThreatOfUno);
			else
				lastInThreatOfUno = null;
			DisableUno();
		}
	}

	/// <summary>
	/// Switches the color of the stack
	/// </summary>
	/// <param name="sender"> The color switcher </param>
	/// <param name="message"> The message containing what new color is set </param>
	private void SwitchColor(AuthenticatedSessionClientHandler sender, string message)
	{
		if (GameplayMessageConstructor.DeconstructColorSwitch(message, out Color color))
		{
			DisableUno();

			players[sender].Cards.Remove(Card.Wild);
			BroadcastExcluding(sender, GameplayMessageConstructor.ConstructRemoveEnemyCard(players[sender]));
			if (players[sender].Cards.Count == 1)
				EnableUno(sender);
			else if (players[sender].Cards.Count == 0)
			{
				PlayerWon(sender);
				return;
			}

			Broadcast(GameplayMessageConstructor.ConstructColorSwitch(color));
			AdvancePlayerTurn();
		}
	}

	/// <summary>
	/// Player wants to place a card
	/// </summary>
	/// <param name="sender"> The player who wants to place a card </param>
	/// <param name="message"> The message containing the card </param>
	private void PlayerPlaceCard(AuthenticatedSessionClientHandler sender, string message)
	{
		if (GameplayMessageConstructor.DeconstructPlaceCard(message, out Card card))
		{
			DisableUno();

			players[sender].Cards.Remove(card);

			BroadcastExcluding(sender, GameplayMessageConstructor.ConstructRemoveEnemyCard(players[sender]));
			if (players[sender].Cards.Count == 1)
				EnableUno(sender);
			else if (players[sender].Cards.Count == 0)
			{
				PlayerWon(sender);
				return;
			}

			UpdateCardOnTop(card);

			if (card == Card.Yellow_Draw || card == Card.Blue_Draw || card == Card.Green_Draw || card == Card.Red_Draw)
				for (int i = 0; i < 2; i++)
					PlayerTakeCard(players.ElementAt(GetNextTurnPlayerIndex()).Key);
			else if (card == Card.Wild_Draw)
				for (int i = 0; i < 4; i++)
					PlayerTakeCard(players.ElementAt(GetNextTurnPlayerIndex()).Key);
			else if (card == Card.Yellow_Reverse || card == Card.Blue_Reverse || card == Card.Green_Reverse || card == Card.Red_Reverse)
				lobbyModel.TurnsDirection *= -1;
			else if (card == Card.Yellow_Skip || card == Card.Blue_Skip || card == Card.Green_Skip || card == Card.Red_Skip)
				lobbyModel.CurrentPlayerTurnIndex = GetNextTurnPlayerIndex();
			else if (card == Card.Yellow_Reverse || card == Card.Blue_Reverse || card == Card.Green_Reverse || card == Card.Red_Reverse)
				lobbyModel.TurnsDirection *= -1;

			AdvancePlayerTurn();
		}
	}

	/// <summary>
	/// Notify all players what card is currently on the top of the stack
	/// </summary>
	/// <param name="onTop"> The card on top of the stack </param>
	private void UpdateCardOnTop(Card onTop)
	{
		for (int i = 0; i < players.Count; i++)
			_ = players.ElementAt(i).Key.TcpClientHandler.WriteMessage(GameplayMessageConstructor.ConstructNewCardOnTop(onTop));
	}

	/// <summary>
	/// Called when a player has won, wraps up the game and updates all player's stats
	/// </summary>
	/// <param name="winner"></param>
	private void PlayerWon(AuthenticatedSessionClientHandler winner)
	{
		lobbyModel.GameStarted = false;

		SqlLiteDatabaseHandler.IncrementWonTimes(winner.Username);
		foreach (AuthenticatedSessionClientHandler lost in players.Keys)
			if (lost != winner)
				SqlLiteDatabaseHandler.IncrementLostTimes(lost.Username);

		BroadcastExcluding(winner, GameplayMessageConstructor.ConstructLostGameMessage(winner.Username));
		_ = winner.TcpClientHandler.WriteMessage(GameplayMessageConstructor.ConstructWonGameMessage());
	}


	/// <summary>
	/// Enables the uno button for all players and saves the player who is in threat
	/// </summary>
	private void EnableUno(AuthenticatedSessionClientHandler inThreat)
	{
		lastInThreatOfUno = inThreat;
		Broadcast(GameplayMessageConstructor.ConstructEnableUno());
	}

	/// <summary>
	/// Disables the uno button for all players
	/// </summary>
	private void DisableUno()
	{
		Broadcast(GameplayMessageConstructor.ConstructDisableUno());
	}

	/// <summary>
	/// Takes a random card from the deck
	/// </summary>
	/// <param name="sender"> The player to take card </param>
	private void PlayerTakeCard(AuthenticatedSessionClientHandler sender)
	{
		int randomCardIndex = new Random().Next(lobbyModel.CardsDeck.Count);
		Card card = lobbyModel.CardsDeck[randomCardIndex];
		lobbyModel.CardsDeck.RemoveAt(randomCardIndex);

		// Send card to player
		_ = sender.TcpClientHandler.WriteMessage(GameplayMessageConstructor.ConstructPlayerTakeCard(card));

		players[sender].Cards.Add(card);
		BroadcastExcluding(sender, GameplayMessageConstructor.ConstructAddEnemyCard(players[sender]));

		// Check if deck empty
		if (lobbyModel.CardsDeck.Count == 0)
		{
			lobbyModel.CardsDeck = CardDeckInitializer.InitializedCardsDeck();

			// Remove last on stack
			lobbyModel.CardsDeck.Remove(lobbyModel.LastCard);
			ShuffleCardsDeck();
		}
	}

	/// <summary>
	/// Gets the next turn index, based on the direction and the current player turn
	/// </summary>
	/// <returns> Next appropriate player's index to have a turn </returns>
	private int GetNextTurnPlayerIndex()
	{
		int nextIndex = lobbyModel.CurrentPlayerTurnIndex + lobbyModel.TurnsDirection;
		if (nextIndex == players.Count)
			return 0;
		else if (nextIndex == -1)
			return players.Count - 1;
		return nextIndex;
	}

	/// <summary>
	/// Advances the player turn by getting the next appropriate player index and send him that its his turn
	/// </summary>
	private void AdvancePlayerTurn()
	{
		lobbyModel.CurrentPlayerTurnIndex = GetNextTurnPlayerIndex();
		_ = players.ElementAt(lobbyModel.CurrentPlayerTurnIndex).Key.TcpClientHandler.WriteMessage(GameplayMessageConstructor.ConstructYourTurn());
	}

	/// <summary>
	/// Shuffles the cards deck
	/// </summary>
	private void ShuffleCardsDeck()
	{
		lobbyModel.CardsDeck = lobbyModel.CardsDeck.OrderBy(x => Guid.NewGuid()).ToList();
	}

	/// <summary>
	/// Called when a player sends a ready or unready message
	/// </summary>
	/// <param name="sender"> The player who sent the message </param>
	/// <param name="message"> The message containing whether ready or not </param>
	private void PlayerReadyUnready(AuthenticatedSessionClientHandler sender, string message)
	{
		if (GameplayMessageConstructor.DeconstructPlayerReadyUnreadyMessage(message, out bool ready))
		{
			players[sender].Ready = ready;
			BroadcastExcluding(sender, GameplayMessageConstructor.ConstructPlayerReadyUnreadyMessage(ready));
			CheckAllReady();
		}
	}

	/// <summary>
	/// Checks if all of the players are ready, and if they are starts the game
	/// Minimum on 2 players to start the game
	/// </summary>
	private void CheckAllReady()
	{
		if (players.Count < 2)
			return;

		bool allReady = true;
		for (int i = 0; i < players.Count; i++)
			if (!players.ElementAt(i).Value.Ready)
				allReady = false;

		if (allReady)
			StartGame();
	}

	/// <summary>
	/// Sets a starting game state and starts the game for all players
	/// </summary>
	private void StartGame()
	{
		ShuffleCardsDeck();
		lobbyModel.GameStarted = true;
		Broadcast(GameplayMessageConstructor.ConstructGameStarted());
		LobbyManager.LobbiesChanged?.Invoke(LobbyManager.JoinableLobbyModels());
		lobbyModel.CurrentPlayerTurnIndex = new Random().Next(lobbyModel.CurrentPlayerCount);

		Card onTop = GetValidStartingCard();
		UpdateCardOnTop(onTop);
		lobbyModel.CardsDeck.Remove(onTop);

		DealAllPlayerCards();
		EnablePlayerTurn();
	}

	/// <summary>
	/// Gets a valid stack starting card
	/// </summary>
	/// <returns> The valid starting card </returns>
	private Card GetValidStartingCard()
	{
		Card chosen = (Card)new Random().Next(Enum.GetValues(typeof(Card)).Length);
		string[] chosenParts = chosen.ToString().Split('_');
		if (chosenParts.Length < 2 || (chosenParts[0] != "Yellow" && chosenParts[0] != "Blue" && chosenParts[0] != "Green" && chosenParts[0] != "Red") || !int.TryParse(chosenParts[1], out _))
			return GetValidStartingCard();

		return chosen;
	}

	/// <summary>
	/// Deals all players 7 starting cards 
	/// </summary>
	private void DealAllPlayerCards()
	{
		for (int i = 0; i < players.Count; i++)
			for (int j = 0; j < 7; j++)
				PlayerTakeCard(players.ElementAt(i).Key);
	}

	/// <summary>
	/// Enable the current player's turn
	/// </summary>
	private void EnablePlayerTurn()
	{
		_ = players.ElementAt(lobbyModel.CurrentPlayerTurnIndex).Key.TcpClientHandler.WriteMessage(GameplayMessageConstructor.ConstructYourTurn());
	}

	/// <summary>
	/// Adds  a player from the lobby
	/// </summary>
	/// <param name="player"> The player to add </param>
	public void AddPlayer(AuthenticatedSessionClientHandler player)
	{
		if (players.ContainsKey(player))
			return;

		lobbyModel.CurrentPlayerCount++;

		players.Add(player, new PlayerGameDataModel() { Id = currentPlayerId, Name = player.Username });
		OnPlayerJoin(player);

		currentPlayerId++;
	}

	/// <summary>
	/// Removes a player from the lobby
	/// </summary>
	/// <param name="player"> The player to remove </param>
	public void RemovePlayer(AuthenticatedSessionClientHandler player)
	{
		if (!players.ContainsKey(player))
			return;

		lobbyModel.CurrentPlayerCount--;

		OnPlayerLeave(player);
		players.Remove(player);
	}

	/// <summary>
	/// Called when a player has connected to the lobby
	/// </summary>
	/// <param name="player"> The player who connected </param>
	private void OnPlayerJoin(AuthenticatedSessionClientHandler player)
	{
		LobbyManager.PlayerJoined?.Invoke();
		LobbyManager.LobbiesChanged?.Invoke(LobbyManager.JoinableLobbyModels());

		if (!lobbyModel.GameStarted)
			PlayerJoinedPregame(player);
	}


	/// <summary>
	/// Called when a player has disconnected from the lobby
	/// </summary>
	/// <param name="player"> The player who disconnected </param>
	private void OnPlayerLeave(AuthenticatedSessionClientHandler player)
	{
		LobbyManager.PlayerLeft?.Invoke();
		LobbyManager.LobbiesChanged?.Invoke(LobbyManager.JoinableLobbyModels());

		if (lobbyModel.GameStarted)
			PlayerLeftGame(player);
		else
			RemovePlayerFromRing(player);
	}

	/// <summary>
	/// Broadcasts a message to the lobby
	/// </summary>
	/// <param name="message"> The message to broadcast </param>
	private void Broadcast(string message)
	{
		foreach (AuthenticatedSessionClientHandler p in players.Keys)
			_ = p.TcpClientHandler.WriteMessage(message);
	}

	/// <summary>
	/// Broadcasts a message to the lobby, exluding 1 player
	/// </summary>
	/// <param name="exclude"> Player to exclude in the broadcast </param>
	/// <param name="message"> The message to broadcast </param>
	private void BroadcastExcluding(AuthenticatedSessionClientHandler exclude, string message)
	{
		foreach (AuthenticatedSessionClientHandler p in players.Keys)
		{
			if (exclude == p)
				continue;

			_ = p.TcpClientHandler.WriteMessage(message);
		}
	}

	/// <summary>
	/// Called when a player has joined the lobby
	/// </summary>
	/// <param name="player"> The player who joined </param>
	private void PlayerJoinedPregame(AuthenticatedSessionClientHandler player)
	{
		foreach (AuthenticatedSessionClientHandler p in players.Keys)
		{
			if (player == p)
				continue;

			_ = player.TcpClientHandler.WriteMessage(GameplayMessageConstructor.ConstructPlayerJoinedMessage(players[p]));
			_ = p.TcpClientHandler.WriteMessage(GameplayMessageConstructor.ConstructPlayerJoinedMessage(players[player]));
		}
	}

	/// <summary>
	/// Called when a player has left the game mid-session
	/// </summary>
	/// <param name="player"> The player who left </param>
	private void PlayerLeftGame(AuthenticatedSessionClientHandler player)
	{
		RemovePlayerFromRing(player);

		if (lobbyModel.CurrentPlayerCount == 1)
			PlayerWon(players.ElementAt(0).Key == player ? players.ElementAt(1).Key : players.ElementAt(0).Key);
		else
			SqlLiteDatabaseHandler.IncrementLostTimes(player.Username);

		if (players.ElementAt(lobbyModel.CurrentPlayerTurnIndex).Key == player)
			AdvancePlayerTurn();
	}

	/// <summary>
	/// Tells all connected players that a player should be removed from their display
	/// </summary>
	/// <param name="player"> The player to remove </param>
	private void RemovePlayerFromRing(AuthenticatedSessionClientHandler player)
	{
		foreach (AuthenticatedSessionClientHandler p in players.Keys)
		{
			if (player == p)
				continue;

			_ = p.TcpClientHandler.WriteMessage(GameplayMessageConstructor.ConstructPlayerLeftMessage(players[player]));
		}
	}
}