namespace Uno.Core.Utilities.MessageHelpers;

internal class MessageConstructor
{
	private const char TypeParametersSeparator = ':';

	public static string ConstructMessage(string type, params string[] args)
	{
		return $"{type}{TypeParametersSeparator}{MessageParameterParser.ConstructParameters(args)}";
	}
	public static bool DeconstructMessage(string data, out string type, out string[] parameters)
	{
		type = "";
		parameters = [];

		string[] parts = data.Split(TypeParametersSeparator);
		if (parts.Length < 2)
			return false;

		type = parts[0];
		parameters = MessageParameterParser.DeconstructParameters(parts[1]);
		return true;
	}
}