namespace Uno.Core.Utilities.CommunicationProtocols.LoginRegister;

public enum RegisterResponse
{
	// Register responses
	None,
	Success,
	RegisterResponse,
	UsernameExists,
	InvalidUsername,
	InvalidPassword,
	InvalidEmail,
	EmailInUse,
	TwoFactorAuthenticationSent,
	TwoFACodeExpired,
	UnknownError,
	Wrong2FACode,
}