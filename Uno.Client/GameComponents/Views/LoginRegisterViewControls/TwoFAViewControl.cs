using GameEngine.Components.UIComponents;
using GameEngine.Core.Components.Objects;
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
/// The 2fa view control
/// </summary>
internal class TwoFAViewControl : UIObject
{
	public UIButton switchToLoginButton;
	private UILabel usernameLabel;
	private UITextBox usernameTextBox;
	private UILabel twoFALabel;
	private UITextBox twoFATextBox;
	private UIButton confirm2FAButton;

	public event Action? OnSuccess;

	public TwoFAViewControl()
	{
		Vector3 fieldScale = new Vector3(0.25f, 0.15f, 1);
		float difference = 0.01f;

		OnSuccess += () => Visible = false;

		// Username label
		usernameLabel = new UILabel();
		usernameLabel.Text = "Username:";
		usernameLabel.Transform.Scale = fieldScale;
		usernameLabel.Transform.Position = new Vector3(-(fieldScale.X + difference), 0.75f, 0);
		Children.Add(usernameLabel);

		// Username text box
		usernameTextBox = new UITextBox();
		usernameTextBox.TextColor = System.Drawing.Color.White;
		usernameTextBox.Transform.Scale = fieldScale;
		usernameTextBox.Transform.Position = new Vector3(fieldScale.X + difference, 0.75f, 0);
		Children.Add(usernameTextBox);

		// 2FA label
		twoFALabel = new UILabel();
		twoFALabel.Text = "Two FA Code:";
		twoFALabel.Transform.Scale = fieldScale;
		twoFALabel.Transform.Position = new Vector3(-(fieldScale.X + difference), 0.25f, 5f);
		Children.Add(twoFALabel);

		// 2FA text box
		twoFATextBox = new UITextBox();
		twoFATextBox.TextColor = System.Drawing.Color.White;
		twoFATextBox.Transform.Scale = fieldScale;
		twoFATextBox.Transform.Position = new Vector3(fieldScale.X + difference, 0.25f, 5f);
		Children.Add(twoFATextBox);

		// Confirm 2FA button
		confirm2FAButton = new UIButton();
		confirm2FAButton.TextColor = System.Drawing.Color.White;
		confirm2FAButton.OnFullClicked += OnConfirm2FAButton;
		confirm2FAButton.Text = "Confirm 2FA";
		confirm2FAButton.Transform.Scale = fieldScale;
		confirm2FAButton.Transform.Position = new Vector3(fieldScale.X + difference, -0.25f, 0);
		Children.Add(confirm2FAButton);

		// Back to login button
		switchToLoginButton = new UIButton();
		switchToLoginButton.TextColor = System.Drawing.Color.White;
		switchToLoginButton.OnFullClicked += () => { ResetTextBoxes(); Visible = false; };
		switchToLoginButton.Text = "Back to login";
		switchToLoginButton.Transform.Scale = fieldScale;
		switchToLoginButton.Transform.Position = new Vector3(-(fieldScale.X + difference), -0.25f, 0);
		Children.Add(switchToLoginButton);
	}

	/// <summary>
	/// Resets the text boxes
	/// </summary>
	private void ResetTextBoxes()
	{
		usernameTextBox.Text = "";
		twoFATextBox.Text = "";
	}

	/// <summary>
	/// Confirms the 2fa code
	/// </summary>
	private async void OnConfirm2FAButton()
	{
		confirm2FAButton.Enabled = false;
		await TwoFAProcedure();
		confirm2FAButton.Enabled = true;
	}

	/// <summary>
	/// Runs the 2fa confirmation procedure
	/// </summary>
	private async Task TwoFAProcedure()
	{

		if (!Factories.ClientFactory.Create(out TcpClientHandler clientHandler))
		{
			confirm2FAButton.Text = "Error creating a TcpClientHandler";
			return;
		}

		if (!InputPredicates.UsernameValid(usernameTextBox.Text))
		{
			confirm2FAButton.Text = "Invalid username";
			return;
		}

		if (!await clientHandler.Connect(IPAddress.Parse(ServerAddresses.LoginRegisterServerAddress), ServerAddresses.LoginRegisterServerPort))
		{
			confirm2FAButton.Text = "Error connecting to server";
			return;
		}

		// Write two fa request to server 
		if (!await clientHandler.WriteMessage(AuthenticationProcessMessageConstructor.Construct2FARequest(usernameTextBox.Text, twoFATextBox.Text)))
			return;

		// Read two fa response from server
		string? responseStr = await clientHandler.ReadMessage();
		if (responseStr is null)
			return;

		if (AuthenticationProcessMessageConstructor.Deconstruct2FAResponse(responseStr, out TwoFAResponse response))
		{
			switch (response)
			{
				case TwoFAResponse.Success:
					confirm2FAButton.Text = "2FA Successful";
					OnSuccess?.Invoke();
					break;
				case TwoFAResponse.Wrong2FACode:
					confirm2FAButton.Text = "Wrong 2FA Code";
					break;
				case TwoFAResponse.TwoFACodeExpired:
					confirm2FAButton.Text = "2FA Code expired";
					break;
				case TwoFAResponse.UnknownError:
					confirm2FAButton.Text = "Unknown error occurred";
					break;
			}
		}

		// Disconnect at the end of the request
		clientHandler.Disconnect();
	}
}