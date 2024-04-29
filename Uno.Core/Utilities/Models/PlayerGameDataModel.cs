using Uno.Core.Utilities.CommunicationProtocols;

namespace Uno.Core.Utilities.Models;

/// <summary>
/// A player game data
/// </summary>
public class PlayerGameDataModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public bool Ready { get; set; }
	public List<Card> Cards { get; set; }

	public PlayerGameDataModel()
	{
		Id = 0;
		Name = "";
		Ready = false;
		Cards = new List<Card>();
	}

	/// <summary>
	/// Construct a parameter list representing the player model
	/// </summary>
	/// <returns> A string array representing the player model </returns>
	public string[] AsParameters()
	{
		return [Id.ToString(), Name, Ready.ToString()];
	}

	/// <summary>
	/// Parse a plyer model from parameters
	/// </summary>
	/// <param name="parsed"> The out parameter parsed from parameters </param>
	/// <param name="parameters"> The parameters </param>
	/// <returns> Whether the parse was successful </returns>
	public static bool ParsePlayerModel(out PlayerGameDataModel parsed, params string[] parameters)
	{
		parsed = new PlayerGameDataModel();

		if (parameters.Length != 3)
			return false;

		if (!int.TryParse(parameters[0], out int id))
			return false;
		if (!bool.TryParse(parameters[2], out bool ready))
			return false;

		parsed.Id = id;
		parsed.Name = parameters[1];
		parsed.Ready = ready;
		return true;
	}

	/// <summary>
	/// Compares 2 player models
	/// </summary>
	/// <param name="other"> The other player model to compare against </param>
	/// <returns> True if the player models have the same relevant values, false otherwise </returns>
	public bool Equals(PlayerGameDataModel other)
	{
		return Id == other.Id &&
			   Name == other.Name &&
			   Ready == other.Ready &&
			   Cards.SequenceEqual(other.Cards);
	}

	public static bool operator ==(PlayerGameDataModel left, PlayerGameDataModel right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(PlayerGameDataModel left, PlayerGameDataModel right)
	{
		return !(left == right);
	}
}