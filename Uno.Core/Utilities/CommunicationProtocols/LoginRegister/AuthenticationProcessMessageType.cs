namespace Uno.Core.Utilities.CommunicationProtocols.LoginRegister;

public enum AuthenticationProcessMessageType
{
	None,

	LoginRequest,
	LoginResponse,

	RegisterRequest,
	RegisterResponse,

	TwoFARequest,
	TwoFAResponse,

	ForgotPasswordRequest,
	ForgotPasswordResponse,

	NotARobotRequest,
	NotARobotResponse,

	AuthenticationRequest,
}