using Uno.Core.Utilities.CommunicationProtocols;

namespace Uno.Core.Utilities.Models;

/// <summary>
/// A lobby model
/// </summary>
public class LobbyModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string HostName { get; set; }
	public int CurrentPlayerCount { get; set; }
	public bool GameStarted { get; set; }
	public int TurnsDirection { get; set; } = 1;
	public int CurrentPlayerTurnIndex { get; set; }
	public List<Card> CardsDeck { get; set; }
	public Card LastCard { get; set; }

	public LobbyModel()
	{
		Id = 0;
		Name = "";
		HostName = "";
		CurrentPlayerCount = 0;
		CardsDeck = CardDeckInitializer.InitializedCardsDeck();
	}

	/// <summary>
	/// Construct a parameter list representing the lobby model
	/// </summary>
	/// <returns> A string array representing the lobby model </returns>
	public string[] AsParameters()
	{
		return [Id.ToString(), Name, HostName, CurrentPlayerCount.ToString()];
	}

	/// <summary>
	/// Parse a lobby model from parameters
	/// </summary>
	/// <param name="parsed"> The out parameter parsed from parameters </param>
	/// <param name="parameters"> The parameters </param>
	/// <returns> Whether the parse was successful </returns>
	public static bool ParseLobbyModel(out LobbyModel parsed, params string[] parameters)
	{
		parsed = new LobbyModel();

		if (parameters.Length != 4)
			return false;

		if (!int.TryParse(parameters[0], out int id))
			return false;
		if (!int.TryParse(parameters[3], out int currentPlayerCount))
			return false;

		parsed.Id = id;
		parsed.Name = parameters[1];
		parsed.HostName = parameters[2];
		parsed.CurrentPlayerCount = currentPlayerCount;
		return true;
	}
}