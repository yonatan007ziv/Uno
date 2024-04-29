namespace Uno.Core.Utilities.CommunicationProtocols;

/// <summary>
/// All of the available card types
/// </summary>
public enum Card
{
	None,
	Yellow_0, Yellow_1, Yellow_2, Yellow_3, Yellow_4, Yellow_5, Yellow_6, Yellow_7, Yellow_8, Yellow_9,
	Blue_0, Blue_1, Blue_2, Blue_3, Blue_4, Blue_5, Blue_6, Blue_7, Blue_8, Blue_9,
	Green_0, Green_1, Green_2, Green_3, Green_4, Green_5, Green_6, Green_7, Green_8, Green_9,
	Red_0, Red_1, Red_2, Red_3, Red_4, Red_5, Red_6, Red_7, Red_8, Red_9,

	Yellow_Draw, Yellow_Reverse, Yellow_Skip,
	Blue_Draw, Blue_Reverse, Blue_Skip,
	Green_Draw, Green_Reverse, Green_Skip,
	Red_Draw, Red_Reverse, Red_Skip,

	// Card cover
	Cover,

	// Not colored
	Wild, Wild_Draw,
}