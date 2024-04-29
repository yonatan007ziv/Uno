namespace Uno.Core.Utilities.CommunicationProtocols.Game;

public enum GameMessage
{
	None,
	PregamePlayerList,
	GameStarted,
	PlayerWon,
	PlayerLost,
	PlayerJoinedPregame,
	PlayerLeftPregame,
	PlayerReadyUnready,
	YourTurn,
	TakeCard,
	PlaceCard,
	CallUno,
	EnableUno,
	DisableUno,
	NewCardOnStack,
	EnemyAddCard,
	EnemyRemoveCard,
	ColorSwitch,
	TakeRandomCard,
}