namespace Uno.Core.Utilities.CommunicationProtocols.LoginRegister;

public enum TwoFAResponse
{
	None,
	Success,
	Wrong2FACode,
	UnknownError,
	InvalidUsername,
	TwoFACodeExpired,
}