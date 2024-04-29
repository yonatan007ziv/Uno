using System.Text;
using Uno.Core.Utilities.CommunicationProtocols.LoginRegister;
using Uno.Core.Utilities.InputValidators;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Server.Components.Database;

namespace Uno.Server.Components.Networking.ClientHandlers;

internal class AuthenticationProcessClientHandler : BaseClientHandler
{
	private readonly bool[] notARobotTiles = new bool[9];

	public override async void StartRead()
	{
		while (true)
		{
			string? message = await TcpClientHandler.ReadMessage();
			if (message is null)
				return;

			await InterpretMessage(message);
		}
	}

	public async Task InterpretMessage(string message)
	{
		await Console.Out.WriteLineAsync($"Got pre-authentication message.");
		if (!AuthenticationProcessMessageConstructor.GetMessageType(message, out AuthenticationProcessMessageType messageType))
			return;

		if (messageType == AuthenticationProcessMessageType.LoginRequest)
			await LoginRequest(message);
		else if (messageType == AuthenticationProcessMessageType.RegisterRequest)
			await RegisterRequest(message);
		else if (messageType == AuthenticationProcessMessageType.TwoFARequest)
			await TwoFARequest(message);
		else if (messageType == AuthenticationProcessMessageType.ForgotPasswordRequest)
			await ForgotPasswordRequestRequest(message);
		else if (messageType == AuthenticationProcessMessageType.NotARobotRequest)
			await NotARobotRequest(message);
	}

	private async Task LoginRequest(string requestData)
	{
		LoginResponse response = LoginResponse.UnknownError;
		string authenticationCode = "";
		if (AuthenticationProcessMessageConstructor.DeconstructLoginRequest(requestData, out string username, out string password))
		{
			if (!SqlLiteDatabaseHandler.UsernameExists(username))
				response = LoginResponse.UsernameDoesNotExist;
			else if (!SqlLiteDatabaseHandler.CheckPassword(username, password))
				response = LoginResponse.WrongPassword;
			else if (!SqlLiteDatabaseHandler.GetEmailConfirmed(username))
			{
				response = LoginResponse.TwoFactorAuthenticationSent;
				Send2FA(SqlLiteDatabaseHandler.GetEmail(username), username, out _);
			}
			else
			{
				authenticationCode = ClientAuthenticator.GenerateAuthenticationToken(username);
				response = LoginResponse.Success;
			}
		}
		await TcpClientHandler.WriteMessage(AuthenticationProcessMessageConstructor.ConstructLoginResponse(response, authenticationCode));
	}

	private async Task RegisterRequest(string requestData)
	{
		RegisterResponse response = RegisterResponse.UnknownError;
		if (AuthenticationProcessMessageConstructor.DeconstructRegisterRequest(requestData, out string username, out string password, out string email))
		{
			if (SqlLiteDatabaseHandler.UsernameExists(username))
				response = RegisterResponse.UsernameExists;
			else if (!InputPredicates.UsernameValid(username))
				response = RegisterResponse.InvalidUsername;
			else if (!InputPredicates.PasswordValid(password))
				response = RegisterResponse.InvalidPassword;
			else if (!InputPredicates.EmailValid(email))
				response = RegisterResponse.InvalidEmail;
			else if (SqlLiteDatabaseHandler.EmailExists(email))
				response = RegisterResponse.EmailInUse;
			else if (!Send2FA(email, username, out byte[] twoFAHash))
				response = RegisterResponse.InvalidEmail;
			else
			{
				response = RegisterResponse.TwoFactorAuthenticationSent;

				// Hash and salt password
				HasherSalter.HashSaltPasswordRandomSalt(password, out byte[] hash, out byte[] salt);

				// Insert user without confirming the email
				SqlLiteDatabaseHandler.InsertUser(username, email, hash, salt, twoFAHash);
			}
		}

		await TcpClientHandler.WriteMessage(AuthenticationProcessMessageConstructor.ConstructRegisterResponse(response));
	}

	private async Task NotARobotRequest(string data)
	{
		Random rand = new Random();
		for (int i = 0; i < notARobotTiles.Length; i++)
			notARobotTiles[i] = rand.Next(3) == 0;
		notARobotTiles[4] = true;

		await TcpClientHandler.WriteMessage(AuthenticationProcessMessageConstructor.ConstructNotARobotResponse(NotARobotResponse.Squares, notARobotTiles));
	}

	private async Task TwoFARequest(string requestData)
	{
		TwoFAResponse result = TwoFAResponse.None;
		if (AuthenticationProcessMessageConstructor.Deconstruct2FARequest(requestData, out string username, out string twoFACode))
		{
			if (!SqlLiteDatabaseHandler.UsernameExists(username))
				result = TwoFAResponse.InvalidUsername;
			else if (!SqlLiteDatabaseHandler.Get2FATime(username, out DateTime twoFASentTime))
				result = TwoFAResponse.UnknownError;
			else if (DateTime.Now.Subtract(twoFASentTime) > TimeSpan.FromMinutes(5))
				result = TwoFAResponse.TwoFACodeExpired;
			else if (CheckAgainstStored2FA(username, twoFACode))
			{
				SqlLiteDatabaseHandler.ValidateEmail(username);
				result = TwoFAResponse.Success;
			}
			else
				result = TwoFAResponse.Wrong2FACode;
		}

		await TcpClientHandler.WriteMessage(AuthenticationProcessMessageConstructor.Construct2FAResponse(result));
	}

	private bool CheckAgainstStored2FA(string username, string twoFACode)
	{
		byte[] storedTwoFAHash = SqlLiteDatabaseHandler.Get2FAHash(username);
		byte[] twoFAHash = HasherSalter.HashArray(Encoding.UTF8.GetBytes(twoFACode));

		if (storedTwoFAHash.Length != twoFAHash.Length)
			return false;
		for (int i = 0; i < twoFAHash.Length; i++)
			if (storedTwoFAHash[i] != twoFAHash[i])
				return false;
		return true;
	}

	private async Task ForgotPasswordRequestRequest(string requestData)
	{

	}

	private bool Send2FA(string email, string username, out byte[] twoFAHash)
	{
		string twoFA = TwoFactorAuthenticationCodeGenerator.Generate2FACode();
		twoFAHash = HasherSalter.HashArray(Encoding.UTF8.GetBytes(twoFA));

		SqlLiteDatabaseHandler.Set2FATime(username, DateTime.Now);
		SqlLiteDatabaseHandler.Set2FAHash(username, twoFAHash);
		return EmailSender.SendEmail(email, "2FA Token", $"Your 2FA token is: {twoFA}");
	}
}