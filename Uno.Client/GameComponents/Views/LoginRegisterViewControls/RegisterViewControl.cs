using GameEngine.Components.UIComponents;
using GameEngine.Core.Components.Objects;
using System.Drawing;
using System.Net;
using System.Numerics;
using Uno.Client.Components;
using Uno.Client.Components.Networking;
using Uno.Core.Utilities.CommunicationProtocols.LoginRegister;
using Uno.Core.Utilities.InputValidators;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Core.Utilities.Networking;

namespace Uno.Client.GameComponents.Views.LoginRegisterViewControls;

/// <summary>
/// The register's view control
/// </summary>
internal class RegisterViewControl : UIObject
{
	public event Action? OnEmailNotConfirmed;

	public UIButton switchToLoginButton;

	private readonly UIButton registerButton;
	private readonly UITextBox usernameTextBox;
	private readonly UITextBox passwordTextBox;
	private readonly UITextBox emailTextBox;

	public RegisterViewControl()
	{
		Vector3 fieldScale = new Vector3(0.25f, 0.15f, 1);
		float difference = 0.01f;

		OnEmailNotConfirmed += () => Visible = false;

		// Username label
		UILabel usernameLabel = new UILabel();
		usernameLabel.Text = "Username:";
		usernameLabel.Transform.Scale = fieldScale;
		usernameLabel.Transform.Position = new Vector3(-(fieldScale.X + difference), 0.75f, 5f);
		Children.Add(usernameLabel);

		// Password label
		UILabel passwordLabel = new UILabel();
		passwordLabel.Text = "Password:";
		passwordLabel.Transform.Scale = fieldScale;
		passwordLabel.Transform.Position = new Vector3(-(fieldScale.X + difference), 0.25f, 5f);
		Children.Add(passwordLabel);

		// Email label
		UILabel emailLabel = new UILabel();
		emailLabel.Text = "Email:";
		emailLabel.Transform.Scale = fieldScale;
		emailLabel.Transform.Position = new Vector3(-(fieldScale.X + difference), -0.25f, 5f);
		Children.Add(emailLabel);

		// Register button
		registerButton = new UIButton();
		registerButton.TextColor = Color.White;
		registerButton.Text = "Register";
		registerButton.Transform.Scale = fieldScale;
		registerButton.Transform.Position = new Vector3(fieldScale.X + difference, -0.75f, 5f);
		Children.Add(registerButton);

		// Username text box
		usernameTextBox = new UITextBox();
		usernameTextBox.TextColor = Color.White;
		usernameTextBox.Transform.Scale = fieldScale;
		usernameTextBox.Transform.Position = new Vector3(fieldScale.X + difference, 0.75f, 5f);
		Children.Add(usernameTextBox);

		// Password text box
		passwordTextBox = new UITextBox();
		passwordTextBox.TextColor = Color.White;
		passwordTextBox.Transform.Scale = fieldScale;
		passwordTextBox.Transform.Position = new Vector3(fieldScale.X + difference, 0.25f, 5f);
		Children.Add(passwordTextBox);

		// Email text box
		emailTextBox = new UITextBox();
		emailTextBox.TextColor = Color.White;
		emailTextBox.Transform.Scale = fieldScale;
		emailTextBox.Transform.Position = new Vector3(fieldScale.X + difference, -0.25f, 5f);
		Children.Add(emailTextBox);

		// Switch button
		switchToLoginButton = new UIButton();
		switchToLoginButton.TextColor = Color.White;
		switchToLoginButton.OnFullClicked += () => { ResetTextBoxes(); Visible = false; };
		switchToLoginButton.Text = "Login?";
		switchToLoginButton.Transform.Scale = fieldScale;
		switchToLoginButton.Transform.Position = new Vector3(-(fieldScale.X + difference), -0.75f, 5f);
		Children.Add(switchToLoginButton);

		// Register button clicked
		registerButton.OnFullClicked += OnRegisterButtonClicked;
	}

	/// <summary>
	/// Resets the text boxes
	/// </summary>
	private void ResetTextBoxes()
	{
		usernameTextBox.Text = "";
		passwordTextBox.Text = "";
		emailTextBox.Text = "";
	}

	/// <summary>
	/// Runs through the register procedure
	/// </summary>
	private async void OnRegisterButtonClicked()
	{
		registerButton.Enabled = false;
		await RegisterProcedure();
		registerButton.Enabled = true;
	}

	/// <summary>
	/// The register procedure
	/// </summary>
	private async Task RegisterProcedure()
	{
		if (!Factories.ClientFactory.Create(out TcpClientHandler clientHandler))
		{
			registerButton.Text = "Error creating a TcpClientHandler";
			return;
		}

		if (!InputPredicates.UsernameValid(usernameTextBox.Text))
		{
			registerButton.Text = "Invalid username";
			return;
		}

		if (!InputPredicates.PasswordValid(passwordTextBox.Text))
		{
			registerButton.Text = "Invalid password";
			return;
		}

		if (!InputPredicates.EmailValid(emailTextBox.Text))
		{
			registerButton.Text = "Invalid email";
			return;
		}

		if (!await clientHandler.Connect(IPAddress.Parse(ServerAddresses.LoginRegisterServerAddress), ServerAddresses.LoginRegisterServerPort))
		{
			registerButton.Text = "Error connecting to server";
			return;
		}

		// Write login request to server 
		if (!await clientHandler.WriteMessage(AuthenticationProcessMessageConstructor.ConstructRegisterRequest(usernameTextBox.Text, passwordTextBox.Text, emailTextBox.Text)))
			return;

		// Read login response from server
		string? responseStr = await clientHandler.ReadMessage();
		if (responseStr is null)
			return;

		if (AuthenticationProcessMessageConstructor.DeconstructRegisterResponse(responseStr, out RegisterResponse response))
		{
			switch (response)
			{
				case RegisterResponse.None:
					registerButton.Text = "Critical: response - None";
					break;
				case RegisterResponse.Success:
					break;
				case RegisterResponse.UnknownError:
					registerButton.Text = "Unknown error occurred";
					break;
				case RegisterResponse.EmailInUse:
					registerButton.Text = "Email already exists";
					break;
				case RegisterResponse.TwoFactorAuthenticationSent:
					OnEmailNotConfirmed?.Invoke();
					registerButton.Text = "Two factor authentication needed";
					break;
			}
		}

		// Disconnect at the end of the request
		clientHandler.Disconnect();
	}
}