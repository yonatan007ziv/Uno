using Uno.Core.Utilities.CommunicationProtocols;

namespace Uno.Core.Utilities;

/// <summary>
/// Used as a way to initialize a card deck
/// </summary>
public class CardDeckInitializer
{
	private static readonly IReadOnlyCollection<Card> startingDeck =
	[
		Card.Yellow_0, Card.Blue_0, Card.Green_0, Card.Red_0,
		Card.Yellow_1, Card.Blue_1, Card.Green_1, Card.Red_1,
		Card.Yellow_2, Card.Blue_2, Card.Green_2, Card.Red_2,
		Card.Yellow_3, Card.Blue_3, Card.Green_3, Card.Red_3,
		Card.Yellow_4, Card.Blue_4, Card.Green_4, Card.Red_4,
		Card.Yellow_5, Card.Blue_5, Card.Green_5, Card.Red_5,
		Card.Yellow_6, Card.Blue_6, Card.Green_6, Card.Red_6,
		Card.Yellow_7, Card.Blue_7, Card.Green_7, Card.Red_7,
		Card.Yellow_8, Card.Blue_8, Card.Green_8, Card.Red_8,
		Card.Yellow_9, Card.Blue_9, Card.Green_9, Card.Red_9,
		Card.Yellow_1, Card.Blue_1, Card.Green_1, Card.Red_1,
		Card.Yellow_2, Card.Blue_2, Card.Green_2, Card.Red_2,
		Card.Yellow_3, Card.Blue_3, Card.Green_3, Card.Red_3,
		Card.Yellow_4, Card.Blue_4, Card.Green_4, Card.Red_4,
		Card.Yellow_5, Card.Blue_5, Card.Green_5, Card.Red_5,
		Card.Yellow_6, Card.Blue_6, Card.Green_6, Card.Red_6,
		Card.Yellow_7, Card.Blue_7, Card.Green_7, Card.Red_7,
		Card.Yellow_8, Card.Blue_8, Card.Green_8, Card.Red_8,
		Card.Yellow_9, Card.Blue_9, Card.Green_9, Card.Red_9,
		Card.Yellow_Draw, Card.Blue_Draw, Card.Green_Draw, Card.Red_Draw,
		Card.Yellow_Draw, Card.Blue_Draw, Card.Green_Draw, Card.Red_Draw,
		Card.Yellow_Skip, Card.Blue_Skip, Card.Green_Skip, Card.Red_Skip,
		Card.Yellow_Skip, Card.Blue_Skip, Card.Green_Skip, Card.Red_Skip,
		Card.Yellow_Reverse, Card.Blue_Reverse, Card.Green_Reverse, Card.Red_Reverse,
		Card.Yellow_Reverse, Card.Blue_Reverse, Card.Green_Reverse, Card.Red_Reverse,
		Card.Wild,Card.Wild,Card.Wild,Card.Wild,
		Card.Wild_Draw,Card.Wild_Draw,Card.Wild_Draw,Card.Wild_Draw,
	];

	/// <summary>
	/// Initialize a new card deck
	/// </summary>
	/// <returns> The new card deck </returns>
	public static List<Card> InitializedCardsDeck()
	{
		return new List<Card>(startingDeck);
	}
}