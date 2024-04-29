using System.Text;

namespace Uno.Core.Utilities.MessageHelpers;

internal class MessageParameterParser
{
	private const char ParametersOpener = '(';
	private const char ParametersCloser = ')';
	private const char ParametersSeparator = ',';

	public static string ConstructParameters(params string[] args)
	{
		StringBuilder builder = new StringBuilder();

		builder.Append(ParametersOpener);
		for (int i = 0; i < args.Length; i++)
		{
			builder.Append(args[i]);
			if (i != args.Length - 1)
				builder.Append(ParametersSeparator);
		}

		builder.Append(ParametersCloser);
		return builder.ToString();
	}

	public static string[] DeconstructParameters(string data)
	{
		return data.Split(ParametersOpener)[1].Split(ParametersCloser)[0].Split(ParametersSeparator);
	}
}