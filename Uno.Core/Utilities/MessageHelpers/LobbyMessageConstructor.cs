using Uno.Core.Utilities.CommunicationProtocols.Lobby;
using Uno.Core.Utilities.MessageHelpers;
using Uno.Core.Utilities.Models;

namespace Uno.Core.Utilities.MessageConstructors;

public static class LobbyMessageConstructor
{
	public static bool GetMessageType(string data, out LobbyMessage messageType)
	{
		messageType = LobbyMessage.None;

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		return Enum.TryParse(type, out messageType);
	}


	public static string ConstructNotifyInLobbySelection()
	{
		return MessageConstructor.ConstructMessage(LobbyMessage.JoinedLobbySelection.ToString());
	}

	public static string ConstructLobbiesListMessage(List<LobbyModel> lobbies)
	{
		List<string> joinedParameters = new List<string>();
		foreach (LobbyModel lobby in lobbies)
			joinedParameters.AddRange(lobby.AsParameters());
		return MessageConstructor.ConstructMessage(LobbyMessage.LobbyList.ToString(), joinedParameters.ToArray());
	}
	public static bool DeconstructLobbiesListMessage(string data, out List<LobbyModel> lobbies)
	{
		lobbies = new List<LobbyModel>();


		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out LobbyMessage requestType) || requestType != LobbyMessage.LobbyList)
			return false;

		int properiesPerModel = 4;
		if (parameters.Length % properiesPerModel != 0)
			return false;

		for (int i = 0; i < parameters.Length / properiesPerModel; i++)
		{
			try
			{
				int startingIndex = i * properiesPerModel;
				if (!LobbyModel.ParseLobbyModel(out LobbyModel parsed, parameters.Skip(startingIndex).Take(properiesPerModel).ToArray()))
					return false;
				lobbies.Add(parsed);
			}
			catch { return false; }
		}
		return true;
	}

	public static string ConstructLobbyCreateRequest(LobbyModel lobbyData)
	{
		return MessageConstructor.ConstructMessage(LobbyMessage.CreateLobby.ToString(), lobbyData.AsParameters());
	}
	public static bool DeconstructLobbyCreateRequest(string data, out LobbyModel createdLobbyModel)
	{
		createdLobbyModel = default;

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out LobbyMessage requestType) || requestType != LobbyMessage.CreateLobby)
			return false;

		return LobbyModel.ParseLobbyModel(out createdLobbyModel, parameters);
	}
	public static string ConstructLobbyCreateResponse(LobbyCreateJoinResponse response, LobbyModel createdLobbyModel)
	{
		string[] parameters = new string[] { response.ToString() }.Concat(createdLobbyModel.AsParameters()).ToArray();
		return MessageConstructor.ConstructMessage(LobbyMessage.CreateLobby.ToString(), parameters);
	}
	public static bool DeconstructLobbyCreateResponse(string data, out LobbyCreateJoinResponse response, out LobbyModel createdLobbyModel)
	{
		response = LobbyCreateJoinResponse.Failed;
		createdLobbyModel = new LobbyModel();

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		// Validate that it is a lobby list message
		if (!Enum.TryParse(type, out LobbyMessage requestType) || requestType != LobbyMessage.CreateLobby)
			return false;

		if (!Enum.TryParse(parameters[0], out response))
			return false;

		if (response == LobbyCreateJoinResponse.Success)
			return LobbyModel.ParseLobbyModel(out createdLobbyModel, parameters.Skip(1).ToArray());

		return true;
	}


	public static string ConstructLobbyJoinRequest(int id)
	{
		return MessageConstructor.ConstructMessage(LobbyMessage.JoinLobby.ToString(), id.ToString());
	}
	public static bool DeconstructLobbyJoinRequest(string data, out int id)
	{
		id = 0;

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out LobbyMessage requestType) || requestType != LobbyMessage.JoinLobby)
			return false;

		return int.TryParse(parameters[0], out id);
	}
	public static string ConstructLobbyJoinResponse(LobbyCreateJoinResponse response)
	{
		return MessageConstructor.ConstructMessage(LobbyMessage.JoinLobby.ToString(), response.ToString());
	}
	public static bool DeconstructLobbyJoinResponse(string data, out LobbyCreateJoinResponse response)
	{
		response = LobbyCreateJoinResponse.Failed;

		if (!MessageConstructor.DeconstructMessage(data, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out LobbyMessage requestType) || requestType != LobbyMessage.JoinLobby)
			return false;

		return Enum.TryParse(parameters[0], out response);
	}

	public static string ConstructStatsRequest(string username)
	{
		return MessageConstructor.ConstructMessage(LobbyMessage.GetStatsRequest.ToString(), username);
	}
	public static bool DeconstructStatsRequest(string message, out string username)
	{
		username = "";

		if (!MessageConstructor.DeconstructMessage(message, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out LobbyMessage requestType) || requestType != LobbyMessage.GetStatsRequest)
			return false;

		if (parameters.Length < 1)
			return false;

		username = parameters[0];
		return true;
	}
	public static string ConstructStatsResponse(int wonTimes, int lostTimes)
	{
		return MessageConstructor.ConstructMessage(LobbyMessage.GetStatsResponse.ToString(), wonTimes.ToString(), lostTimes.ToString());
	}
	public static bool DeconstructStatsResponse(string message, out int wonTimes, out int lostTimes)
	{
		wonTimes = 0;
		lostTimes = 0;

		if (!MessageConstructor.DeconstructMessage(message, out string type, out string[] parameters))
			return false;

		if (!Enum.TryParse(type, out LobbyMessage requestType) || requestType != LobbyMessage.GetStatsResponse)
			return false;

		if (!int.TryParse(parameters[0], out wonTimes))
			return false;

		if (!int.TryParse(parameters[1], out lostTimes))
			return false;

		return true;
	}
}