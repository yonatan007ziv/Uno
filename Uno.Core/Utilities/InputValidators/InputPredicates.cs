using Uno.Core.Utilities.MessageConstructors;

namespace Uno.Core.Utilities.InputValidators;

public static class InputPredicates
{
	private static string[] passwordCharacterAsciiRanges =
	{
		// Required ascii ranges
		"AZ",
		"az",
		"09",
	};

	public static bool UsernameValid(string username)
		=> username.Length > 0 && !username.Contains(AuthenticationMessageConstructor.separator);

	public static bool PasswordValid(string password)
	{
		if (password.Length < 8) // Minimum 8 characters
			return false;

		if (password.Contains(AuthenticationMessageConstructor.separator)) // Password contains the character used for internal separation
			return false;

		// Check if the password contains at least one character from each specified range
		foreach (var range in passwordCharacterAsciiRanges)
			if (!password.Any(c => range[0] <= c && c <= range[1]))
				return false;

		// Ensure that all characters in the password are within the allowed ranges
		if (!password.All(c => passwordCharacterAsciiRanges.Any(range => range[0] <= c && c <= range[1])))
			return false;

		return true;
	}

	public static bool EmailValid(string email)
		=> email.Length > 3 && email.Contains('@') && !email.Contains(AuthenticationMessageConstructor.separator);
}