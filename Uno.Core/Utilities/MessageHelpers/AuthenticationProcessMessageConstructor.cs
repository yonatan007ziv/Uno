using System.Text;
using Uno.Core.Utilities.CommunicationProtocols.LoginRegister;

namespace Uno.Core.Utilities.MessageConstructors;

public static class AuthenticationProcessMessageConstructor
{
	public static bool GetMessageType(string data, out AuthenticationProcessMessageType type)
	{
		return Enum.TryParse(data.Split(':')[0], out type);
	}

	#region Login
	public static string ConstructLoginRequest(string username, string password)
	{
		return $"{AuthenticationProcessMessageType.LoginRequest}:({username},{password})";
	}
	public static bool DeconstructLoginRequest(string data, out string username, out string password)
	{
		username = "";
		password = "";

		// Validate that it is a login request
		if (!Enum.TryParse(data.Split(':')[0], out AuthenticationProcessMessageType requestType) || requestType != AuthenticationProcessMessageType.LoginRequest)
			return false;

		string[] parts = data.Split('(')[1].Split(')')[0].Split(',');
		if (parts.Length < 2)
			return false;

		username = parts[0];
		password = parts[1];
		return true;
	}
	public static string ConstructLoginResponse(LoginResponse response, string authenticationToken)
	{
		return $"{AuthenticationProcessMessageType.LoginResponse}:({response},{authenticationToken})";
	}
	public static bool DeconstructLoginResponse(string data, out LoginResponse response, out string authenticationToken)
	{
		response = LoginResponse.None;
		authenticationToken = "";

		// Validate that it is a login response
		if (!Enum.TryParse(data.Split(':')[0], out AuthenticationProcessMessageType requestType) || requestType != AuthenticationProcessMessageType.LoginResponse)
			return false;

		string[] parts = data.Split('(')[1].Split(')')[0].Split(',');

		if (!Enum.TryParse(parts[0], out response))
			return false;

		if (parts.Length > 1)
			authenticationToken = parts[1];

		return true;
	}
	#endregion

	#region Register
	public static string ConstructRegisterRequest(string username, string password, string email)
	{
		return $"{AuthenticationProcessMessageType.RegisterRequest}:({username},{password},{email})";
	}
	public static bool DeconstructRegisterRequest(string data, out string username, out string password, out string email)
	{
		username = "";
		password = "";
		email = "";

		// Validate that it is a register request
		if (!Enum.TryParse(data.Split(':')[0], out AuthenticationProcessMessageType requestType) || requestType != AuthenticationProcessMessageType.RegisterRequest)
			return false;

		string[] parts = data.Split('(')[1].Split(')')[0].Split(',');
		if (parts.Length < 3)
			return false;

		username = parts[0];
		password = parts[1];
		email = parts[2];
		return true;
	}
	public static string ConstructRegisterResponse(RegisterResponse response)
	{
		return $"{AuthenticationProcessMessageType.RegisterResponse}:({response})";
	}
	public static bool DeconstructRegisterResponse(string data, out RegisterResponse response)
	{
		response = RegisterResponse.None;

		// Validate that it is a login response
		if (!Enum.TryParse(data.Split(':')[0], out AuthenticationProcessMessageType requestType) || requestType != AuthenticationProcessMessageType.RegisterResponse)
			return false;

		string responseStr = data.Split('(')[1].Split(')')[0];
		if (!Enum.TryParse(responseStr, out response))
			return false;

		return true;
	}
	#endregion

	#region 2FA
	public static string Construct2FARequest(string username, string twoFACode)
	{
		return $"{AuthenticationProcessMessageType.TwoFARequest}:({username},{twoFACode})";
	}
	public static bool Deconstruct2FARequest(string data, out string username, out string twoFACode)
	{
		username = "";
		twoFACode = "";

		// Validate that it is a 2fa request
		if (!Enum.TryParse(data.Split(':')[0], out AuthenticationProcessMessageType requestType) || requestType != AuthenticationProcessMessageType.TwoFARequest)
			return false;

		string[] parts = data.Split('(')[1].Split(')')[0].Split(',');
		if (parts.Length < 2)
			return false;

		username = parts[0];
		twoFACode = parts[1];
		return true;
	}
	public static string Construct2FAResponse(TwoFAResponse response)
	{
		return $"{AuthenticationProcessMessageType.TwoFAResponse}:({response})";
	}
	public static bool Deconstruct2FAResponse(string data, out TwoFAResponse response)
	{
		response = TwoFAResponse.None;

		// Validate that it is a login response
		if (!Enum.TryParse(data.Split(':')[0], out AuthenticationProcessMessageType requestType) || requestType != AuthenticationProcessMessageType.TwoFAResponse)
			return false;

		string responseStr = data.Split('(')[1].Split(')')[0];
		if (!Enum.TryParse(responseStr, out response))
			return false;

		return true;
	}
	#endregion

	#region Not a Robot
	public static string ConstructNotARobotRequest()
	{
		return $"{AuthenticationProcessMessageType.NotARobotRequest}:()";
	}
	public static bool DeconstructNotARobotRequest(string data)
	{
		// Validate that it is a register request
		if (!Enum.TryParse(data.Split(':')[0], out AuthenticationProcessMessageType requestType) || requestType != AuthenticationProcessMessageType.NotARobotRequest)
			return false;

		return true;
	}
	public static string ConstructNotARobotResponse(NotARobotResponse response, bool[]? squares = null)
	{
		if (squares is null)
			return $"{AuthenticationProcessMessageType.NotARobotResponse}:({response})";

		StringBuilder builder = new StringBuilder($"{AuthenticationProcessMessageType.NotARobotResponse}:({response},");

		for (int i = 0; i < squares.Length; i++)
		{
			builder.Append(squares[i].ToString());
			if (i != squares.Length - 1)
				builder.Append(',');
		}

		builder.Append(')');
		return builder.ToString();
	}
	public static bool DeconstructNotARobotResponse(string data, out NotARobotResponse response, out bool[] squares)
	{
		response = NotARobotResponse.None;
		squares = null!;

		// Validate that it is a login response
		if (!Enum.TryParse(data.Split(':')[0], out AuthenticationProcessMessageType requestType) || requestType != AuthenticationProcessMessageType.NotARobotResponse)
			return false;

		string responseStr = data.Split('(')[1].Split(',')[0];
		if (!Enum.TryParse(responseStr, out response))
			return false;

		if (response == NotARobotResponse.Squares)
		{
			string squareData = data.Split(',')[1].Split(')')[0];

			squares = new bool[9];
			try
			{
				for (int i = 0; i < 9; i++)
					squares[i] = true.ToString() == data.Split('(')[1].Split(')')[0].Split(',')[i + 1];
			}
			catch { return false; }
		}

		return true;
	}
	#endregion
}