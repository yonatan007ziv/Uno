using System.Drawing;
using Uno.Core.Utilities.CommunicationProtocols;
using Uno.Core.Utilities.CommunicationProtocols.Game;
using Uno.Core.Utilities.MessageHelpers;
using Uno.Core.Utilities.Models;

namespace Uno.Core.Utilities.MessageConstructors;

public class GameplayMessageConstructor
{
	public static bool GetMessageType(string data, out GameMessage messageType)
	{
		messageType = GameMessage.None;
		if (!MessageConstructor.DeconstructMessage(data, out string type, out _))
			return false;

		return Enum.TryParse(type, out messageType);
	}

	public static string ConstructPlayerJoinedMessage(PlayerGameDataModel player)
	{
		return MessageConstructor.ConstructMessage(GameMessage.PlayerJoinedPregame.ToString(), player.AsParameters());
	}
	public static bool DeconstructPlayerJoinedMessage(string data, out PlayerGameDataModel player)
	{
		player = default!;

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage messageType) || messageType != GameMessage.PlayerJoinedPregame)
			return false;

		return PlayerGameDataModel.ParsePlayerModel(out player, parameters);
	}

	public static string ConstructUnoCall()
	{
		return MessageConstructor.ConstructMessage(GameMessage.CallUno.ToString());
	}

	public static string ConstructPlayerLeftMessage(PlayerGameDataModel player)
	{
		return MessageConstructor.ConstructMessage(GameMessage.PlayerLeftPregame.ToString(), player.AsParameters());
	}
	public static bool DeconstructPlayerLeftMessage(string data, out PlayerGameDataModel player)
	{
		player = default!;

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage messageType) || messageType != GameMessage.PlayerLeftPregame)
			return false;

		return PlayerGameDataModel.ParsePlayerModel(out player, parameters);
	}

	public static string ConstructPlayerReadyUnreadyMessage(bool ready)
	{
		return MessageConstructor.ConstructMessage(GameMessage.PlayerReadyUnready.ToString(), ready.ToString());
	}
	public static bool DeconstructPlayerReadyUnreadyMessage(string data, out bool ready)
	{
		ready = default;

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage messageType) || messageType != GameMessage.PlayerReadyUnready)
			return false;

		return bool.TryParse(parameters[0], out ready);
	}

	public static string ConstructGameStarted()
	{
		return MessageConstructor.ConstructMessage(GameMessage.GameStarted.ToString());
	}
	public static bool DeconstructGameStarted(string data)
	{
		if (!MessageConstructor.DeconstructMessage(data, out string type, out _))
			return false;

		return Enum.TryParse(type, out GameMessage messageType) && messageType == GameMessage.GameStarted;
	}

	public static string ConstructYourTurn()
	{
		return MessageConstructor.ConstructMessage(GameMessage.YourTurn.ToString());
	}
	public static bool DeconstructYourTurn(string data)
	{
		if (!MessageConstructor.DeconstructMessage(data, out string type, out _))
			return false;

		return Enum.TryParse(type, out GameMessage messageType) && messageType == GameMessage.YourTurn;
	}

	public static string ConstructPlaceCard(Card card)
	{
		return MessageConstructor.ConstructMessage(GameMessage.PlaceCard.ToString(), card.ToString());
	}
	public static bool DeconstructPlaceCard(string data, out Card card)
	{
		card = Card.None;
		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage messageType) && messageType != GameMessage.PlaceCard)
			return false;

		return Enum.TryParse(parameters[0], out card);
	}

	public static string ConstructWonGameMessage()
	{
		return MessageConstructor.ConstructMessage(GameMessage.PlayerWon.ToString());
	}
	public static bool DeconstructWonGameMessage(string data)
	{
		if (!MessageConstructor.DeconstructMessage(data, out string type, out _))
			return false;

		return Enum.TryParse(type, out GameMessage messageType) && messageType == GameMessage.PlayerWon;
	}

	public static string ConstructLostGameMessage(string winnerName)
	{
		return MessageConstructor.ConstructMessage(GameMessage.PlayerLost.ToString(), winnerName);
	}
	public static bool DeconstructLostGameMessage(string data, out string winnerName)
	{
		winnerName = "";
		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage messageType) && messageType == GameMessage.PlayerLost)
			return false;

		if (parameters.Length < 1)
			return false;

		winnerName = parameters[0];
		return true;
	}

	public static string ConstructAddEnemyCard(PlayerGameDataModel player)
	{
		return MessageConstructor.ConstructMessage(GameMessage.EnemyAddCard.ToString(), player.AsParameters());
	}
	public static bool DeconstructAddEnemyCard(string data, out PlayerGameDataModel playerGameDataModel)
	{
		playerGameDataModel = new PlayerGameDataModel();

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage messageType) && messageType == GameMessage.EnemyAddCard)
			return false;

		return PlayerGameDataModel.ParsePlayerModel(out playerGameDataModel, parameters);
	}

	public static string ConstructRemoveEnemyCard(PlayerGameDataModel player)
	{
		return MessageConstructor.ConstructMessage(GameMessage.EnemyRemoveCard.ToString(), player.AsParameters());
	}
	public static bool DeconstructRemoveEnemyCard(string data, out PlayerGameDataModel playerGameDataModel)
	{
		playerGameDataModel = new PlayerGameDataModel();

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage messageType) && messageType == GameMessage.EnemyRemoveCard)
			return false;

		return PlayerGameDataModel.ParsePlayerModel(out playerGameDataModel, parameters);
	}

	public static string ConstructEnableUno()
	{
		return MessageConstructor.ConstructMessage(GameMessage.EnableUno.ToString());
	}
	public static bool DeconstructEnableUno(string data)
	{
		if (!MessageConstructor.DeconstructMessage(data, out string type, out _))
			return false;

		return Enum.TryParse(type, out GameMessage message) && message == GameMessage.EnableUno;
	}

	public static string ConstructDisableUno()
	{
		return MessageConstructor.ConstructMessage(GameMessage.DisableUno.ToString());
	}
	public static bool DeconstructDisableUno(string data)
	{
		if (!MessageConstructor.DeconstructMessage(data, out string type, out _))
			return false;

		return Enum.TryParse(type, out GameMessage message) && message != GameMessage.DisableUno;
	}

	public static string ConstructTakeRandomCard()
	{
		return MessageConstructor.ConstructMessage(GameMessage.TakeRandomCard.ToString());
	}

	public static string ConstructPlayerTakeCard(Card card)
	{
		return MessageConstructor.ConstructMessage(GameMessage.TakeCard.ToString(), card.ToString());
	}
	public static bool DeconstructPlayerTakeCard(string data, out Card card)
	{
		card = Card.None;
		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage message) || message != GameMessage.TakeCard)
			return false;

		return Enum.TryParse(parameters[0], out card);
	}

	public static string ConstructNewCardOnTop(Card card)
	{
		return MessageConstructor.ConstructMessage(GameMessage.NewCardOnStack.ToString(), card.ToString());
	}
	public static bool DeconstructNewCardOnTop(string data, out Card card)
	{
		card = Card.None;
		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage message) || message != GameMessage.NewCardOnStack)
			return false;

		return Enum.TryParse(parameters[0], out card);
	}

	public static string ConstructColorSwitch(Color color)
	{
		return MessageConstructor.ConstructMessage(GameMessage.ColorSwitch.ToString(), color.ToKnownColor().ToString());
	}
	public static bool DeconstructColorSwitch(string data, out Color color)
	{
		color = Color.White;
		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out GameMessage message) || message != GameMessage.ColorSwitch)
			return false;

		color = Color.FromName(parameters[0]);
		return true;
	}
}