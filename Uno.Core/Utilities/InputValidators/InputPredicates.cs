using Uno.Core.Utilities.MessageConstructors;

namespace Uno.Core.Utilities.InputValidators;

public static class InputPredicates
{
	public static bool UsernameValid(string username)
		=> username.Length > 0 && !username.Contains(AuthenticationMessageConstructor.separator);

	public static bool PasswordValid(string password)
		=> password.Length > 3 && password.All(c => c <= 127) && !password.Contains(AuthenticationMessageConstructor.separator);

	public static bool EmailValid(string email)
		=> email.Length > 3 && email.Contains('@') && !email.Contains(AuthenticationMessageConstructor.separator);
}