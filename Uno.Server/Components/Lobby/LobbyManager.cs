using Uno.Core.Utilities;
using Uno.Core.Utilities.Models;
using Uno.Server.Components.Networking.ClientHandlers;

namespace Uno.Server.Components.Lobby;

/// <summary>
/// Handles redirecting players to lobbies, creating and removing lobbies
/// </summary>
internal class LobbyManager
{
	private static readonly Dictionary<int, LobbyHandler> lobbies = new Dictionary<int, LobbyHandler>();
	private static int lobbyId;

	public static Action<List<LobbyModel>>? LobbiesChanged { get; set; }
	public static Action? PlayerJoined { get; set; }
	public static Action? PlayerLeft { get; set; }

	static LobbyManager()
	{
		PlayerLeft += ClearEmptyLobbies;
	}

	/// <summary>
	/// Clears out any lobby with the player count set to 0
	/// </summary>
	private static void ClearEmptyLobbies()
	{
		List<int> keysList = lobbies.Keys.ToList();
		List<int> removedKeys = new List<int>();

		for (int i = 0; i < keysList.Count; i++)
			if (lobbies[keysList[i]].lobbyModel.CurrentPlayerCount == 0)
				removedKeys.Add(keysList[i]);

		for (int i = 0; i < removedKeys.Count; i++)
			lobbies.Remove(removedKeys[i]);
	}

	/// <summary>
	/// Performs a calculation to check what lobbies are joinable from all of the lobbies
	/// </summary>
	/// <returns> A list representing all joinable lobbies </returns>
	public static List<LobbyModel> JoinableLobbyModels()
	{
		IEnumerable<LobbyModel> models = lobbies.Values.Select(handler => handler.lobbyModel);
		IEnumerable<LobbyModel> notFullLobbies = models.Where(lobbyModel => lobbyModel.CurrentPlayerCount < GameConstants.MaxPlayerCount);
		IEnumerable<LobbyModel> notStartedLobbies = notFullLobbies.Where(lobbyModel => !lobbyModel.GameStarted);

		return notStartedLobbies.ToList();
	}

	/// <summary>
	/// Checks if a lobby exists by id
	/// </summary>
	/// <param name="id"> The id to check </param>
	/// <returns> True if it exists, false otherwise </returns>
	public static bool LobbyIdExists(int id)
		=> lobbies.ContainsKey(id);

	/// <summary>
	/// Checks if it's possible to add another player to the requested lobby
	/// </summary>
	/// <param name="lobbyId"> The lobby's id </param>
	/// <returns> True if another player can join, false otherwise </returns>
	public static bool CanAddPlayerToLobby(int lobbyId)
	{
		return lobbies.TryGetValue(lobbyId, out LobbyHandler? joinedLobby) || joinedLobby!.LobbyFull();
	}

	/// <summary>
	/// Adds a player to the lobby
	/// </summary>
	/// <param name="player"> The player to add to the lobby </param>
	/// <param name="lobbyId"> The lobby id to join </param>
	/// <returns> Returns the joined lobby </returns>
	public static LobbyHandler AddPlayerToLobby(AuthenticatedSessionClientHandler player, int lobbyId)
	{
		lobbies[lobbyId].AddPlayer(player);
		return lobbies[lobbyId];
	}

	/// <summary>
	/// Creates a lobby according to the given model
	/// </summary>
	/// <param name="createLobbyModel"> The model to create the lobby after </param>
	public static void CreateLobby(ref LobbyModel createLobbyModel)
	{
		createLobbyModel.Id = lobbyId++;
		lobbies.Add(createLobbyModel.Id, new LobbyHandler(createLobbyModel));
		LobbiesChanged?.Invoke(JoinableLobbyModels());
	}
}