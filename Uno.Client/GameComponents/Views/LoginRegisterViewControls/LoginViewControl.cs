using GameEngine.Components.UIComponents;
using GameEngine.Core.Components.Objects;
using System.Drawing;
using System.Net;
using System.Numerics;
using Uno.Client.Components;
using Uno.Client.Components.Networking;
using Uno.Core.Utilities;
using Uno.Core.Utilities.CommunicationProtocols.LoginRegister;
using Uno.Core.Utilities.InputValidators;
using Uno.Core.Utilities.MessageConstructors;

namespace Uno.Client.GameComponents.Views.LoginRegisterViewControls;

/// <summary>
/// The login's view control
/// </summary>
internal class LoginViewControl : UIObject
{
	public event Action? OnSuccessfulLogin;
	public event Action? OnEmailNotConfirmed;

	public UIButton switchToRegisterButton;
	private UITextBox usernameTextBox;
	private UITextBox passwordTextBox;
	public UILabel resultLabel;
	private UIButton loginButton;

	public LoginViewControl()
	{
		Vector3 fieldScale = new Vector3(0.25f, 0.15f, 1);
		float difference = 0.01f;

		OnSuccessfulLogin += () => Visible = false;
		OnEmailNotConfirmed += () => Visible = false;

		// Username label
		UILabel usernameLabel = new UILabel();
		usernameLabel.Text = "Username:";
		usernameLabel.Transform.Scale = fieldScale;
		usernameLabel.Transform.Position = new Vector3(-(fieldScale.X + difference), 0.75f, 0);
		Children.Add(usernameLabel);

		// Password label
		UILabel passwordLabel = new UILabel();
		passwordLabel.Text = "Password:";
		passwordLabel.Transform.Scale = fieldScale;
		passwordLabel.Transform.Position = new Vector3(-(fieldScale.X + difference), 0.25f, 0);
		Children.Add(passwordLabel);

		// Username text box
		usernameTextBox = new UITextBox();
		usernameTextBox.TextColor = Color.White;
		usernameTextBox.Transform.Scale = fieldScale;
		usernameTextBox.Transform.Position = new Vector3(fieldScale.X + difference, 0.75f, 0);
		Children.Add(usernameTextBox);

		// Password text box
		passwordTextBox = new UITextBox();
		passwordTextBox.TextColor = Color.White;
		passwordTextBox.Transform.Scale = fieldScale;
		passwordTextBox.Transform.Position = new Vector3(fieldScale.X + difference, 0.25f, 0);
		Children.Add(passwordTextBox);

		// Result label
		resultLabel = new UILabel();
		resultLabel.Transform.Scale = fieldScale;
		resultLabel.Transform.Position = new Vector3(0, -0.1f, 0);
		Children.Add(resultLabel);

		// Login button
		loginButton = new UIButton();
		loginButton.TextColor = Color.White;
		loginButton.Text = "Login";
		loginButton.Transform.Scale = fieldScale;
		loginButton.Transform.Position = new Vector3(fieldScale.X + difference, -0.45f, 0);
		Children.Add(loginButton);

		// Switch button
		switchToRegisterButton = new UIButton();
		switchToRegisterButton.TextColor = Color.White;
		switchToRegisterButton.OnFullClicked += () => { ResetView(); Visible = false; };
		switchToRegisterButton.Text = "Register?";
		switchToRegisterButton.Transform.Scale = fieldScale;
		switchToRegisterButton.Transform.Position = new Vector3(-(fieldScale.X + difference), -0.45f, 5f);
		Children.Add(switchToRegisterButton);

		// Login button clicked
		loginButton.OnFullClicked += OnLoginButtonClicked;
	}

	/// <summary>
	/// Resets the text boxes
	/// </summary>
	private void ResetView()
	{
		usernameTextBox.Text = "";
		passwordTextBox.Text = "";
		resultLabel.Text = "";
	}

	/// <summary>
	/// Runs through the login procedure
	/// </summary>
	private async void OnLoginButtonClicked()
	{
		loginButton.Enabled = false;
		await LoginProcedure();
		loginButton.Enabled = true;
	}

	/// <summary>
	/// The login procedure
	/// </summary>
	private async Task LoginProcedure()
	{
		if (!Factories.ClientFactory.Create(out TcpClientHandler clientHandler))
		{
			resultLabel.Text = "Error creating a TcpClientHandler";
			return;
		}

		if (!InputPredicates.UsernameValid(usernameTextBox.Text))
		{
			resultLabel.Text = "Invalid username";
			return;
		}

		if (!InputPredicates.PasswordValid(passwordTextBox.Text))
		{
			resultLabel.Text = "Invalid password";
			return;
		}

		if (!await clientHandler.Connect(IPAddress.Parse(DevConstants.LoginRegisterServerAddress), DevConstants.LoginRegisterServerPort))
		{
			resultLabel.Text = "Error connecting to server";
			return;
		}

		// Write login request to server
		if (!await clientHandler.WriteMessage(AuthenticationProcessMessageConstructor.ConstructLoginRequest(usernameTextBox.Text, passwordTextBox.Text)))
			return;

		// Read login response from server
		string? responseStr = await clientHandler.ReadMessage();
		if (responseStr is null)
			return;

		clientHandler.Disconnect();

		bool success = false, twoFA = false;
		if (AuthenticationProcessMessageConstructor.DeconstructLoginResponse(responseStr, out LoginResponse response, out string authenticationToken))
		{
			switch (response)
			{
				case LoginResponse.None:
					resultLabel.Text = "Critical: response - None";
					break;
				case LoginResponse.Success:
					resultLabel.Text = "Success, Confirm not a Robot";
					success = true;
					break;
				case LoginResponse.UsernameDoesNotExist:
					resultLabel.Text = "Username does not exist";
					break;
				case LoginResponse.WrongPassword:
					resultLabel.Text = "Wrong password";
					break;
				case LoginResponse.TwoFactorAuthenticationSent:
					resultLabel.Text = "Two factor authentication needed";
					twoFA = true;
					break;
			}
		}
		else
			resultLabel.Text = "Invalid message-type received";

		if (success)
		{
			SessionHolder.Username = usernameTextBox.Text;
			SessionHolder.AuthenticationToken = authenticationToken;

			ResetView();
			OnSuccessfulLogin?.Invoke();
		}
		else if (twoFA)
		{
			ResetView();
			OnEmailNotConfirmed?.Invoke();
		}
	}
}