namespace Uno.Core.Utilities.CommunicationProtocols.Lobby;

public enum LobbyMessage
{
	None,
	LobbyList,
	JoinedLobbySelection,
	CreateLobby,
	JoinLobby,
	GetStatsRequest,
	GetStatsResponse,
}