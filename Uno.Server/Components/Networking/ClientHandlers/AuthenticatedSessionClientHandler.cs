using Uno.Core.Utilities.CommunicationProtocols.Lobby;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Core.Utilities.Models;
using Uno.Server.Components.Database;
using Uno.Server.Components.Lobby;

namespace Uno.Server.Components.Networking.ClientHandlers;

internal class AuthenticatedSessionClientHandler : BaseClientHandler
{
	public string Username = "";
	private bool authenticated = false;
	private LobbyHandler? joinedLobby;
	private bool connected;

	public override async void StartRead()
	{
		connected = true;
		while (true)
		{
			string? message = await TcpClientHandler.ReadMessage();
			if (message is null)
				break;

			if (AuthenticationMessageConstructor.DeconstructAuthenticationRequest(message, ref Username, out string authenticationKey))
			{
				authenticated = ClientAuthenticator.CheckAuthenticationToken(Username, authenticationKey);
				continue;
			}

			if (!authenticated)
				continue;

			if (joinedLobby is not null)
				await joinedLobby.InterpretMessage(this, message);
			else
				await InterpretMessage(message);
		}

		joinedLobby?.RemovePlayer(this);
		connected = false;
	}

	public async Task InterpretMessage(string message)
	{
		await Console.Out.WriteLineAsync($"Got authenticated message from \'{Username}\': {message}");
		if (!LobbyMessageConstructor.GetMessageType(message, out LobbyMessage messageType))
			return;

		if (messageType == LobbyMessage.JoinedLobbySelection)
			JoinedLobbySelection();
		else if (messageType == LobbyMessage.CreateLobby)
			CreateLobbyRequest(message);
		else if (messageType == LobbyMessage.JoinLobby)
			JoinLobbyRequest(message);
		else if (messageType == LobbyMessage.GetStatsRequest)
			SendStats(message);
	}

	private void SendStats(string message)
	{
		if (!LobbyMessageConstructor.DeconstructStatsRequest(message, out string username))
			return;

		SqlLiteDatabaseHandler.GetStats(username, out int wonTimes, out int lostTimes);
		_ = TcpClientHandler.WriteMessage(LobbyMessageConstructor.ConstructStatsResponse(wonTimes, lostTimes));
	}

	private void JoinLobbyRequest(string message)
	{
		LobbyCreateJoinResponse response = LobbyCreateJoinResponse.Failed;
		if (LobbyMessageConstructor.DeconstructLobbyJoinRequest(message, out int lobbyId))
			if (LobbyManager.CanAddPlayerToLobby(lobbyId))
				response = LobbyCreateJoinResponse.Success;

		_ = TcpClientHandler.WriteMessage(LobbyMessageConstructor.ConstructLobbyJoinResponse(response));
		if (response == LobbyCreateJoinResponse.Success)
			joinedLobby = LobbyManager.AddPlayerToLobby(this, lobbyId);
	}

	private void JoinedLobbySelection()
	{
		LobbyManager.LobbiesChanged += UpdateLobbyList;
		SendLobbiesList(LobbyManager.JoinableLobbyModels());
	}

	private void UpdateLobbyList(List<LobbyModel> lobbies)
	{
		if (!connected)
			LobbyManager.LobbiesChanged -= UpdateLobbyList;

		SendLobbiesList(lobbies);
	}

	private void SendLobbiesList(List<LobbyModel> lobbies)
	{
		_ = TcpClientHandler.WriteMessage(LobbyMessageConstructor.ConstructLobbiesListMessage(lobbies));
	}

	private async void CreateLobbyRequest(string message)
	{
		LobbyCreateJoinResponse response = LobbyCreateJoinResponse.Failed;
		if (LobbyMessageConstructor.DeconstructLobbyCreateRequest(message, out LobbyModel createdLobby))
		{
			LobbyManager.CreateLobby(ref createdLobby);
			response = LobbyCreateJoinResponse.Success;
		}
		await TcpClientHandler.WriteMessage(LobbyMessageConstructor.ConstructLobbyCreateResponse(response, createdLobby));
	}
}