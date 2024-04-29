namespace Uno.Core.Utilities.CommunicationProtocols.LoginRegister;

public enum LoginResponse
{
	None,
	UnknownError,
	Success,
	UsernameDoesNotExist,
	WrongPassword,
	TwoFactorAuthenticationSent,
}