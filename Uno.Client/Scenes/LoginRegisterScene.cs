using GameEngine.Components;
using GameEngine.Core.Components;
using Uno.Client.GameComponents.Views.LoginRegisterViewControls;

namespace Uno.Client.Scenes;

/// <summary>
/// The login register scene
/// </summary>
internal class LoginRegisterScene : Scene
{
	private readonly LoginViewControl _loginView;
	private readonly RegisterViewControl _registerView;
	private readonly TwoFAViewControl _twoFAView;
	private readonly NotARobotViewControl _notARobotView;

	public LoginRegisterScene()
	{
		// Add ui camera
		UICameras.Add((new UICamera(), new ViewPort(0.5f, 0.5f, 1, 1)));

		// Login view
		_loginView = new LoginViewControl();
		_loginView.OnSuccessfulLogin += SwitchToNotARobot;
		_loginView.OnEmailNotConfirmed += SwitchTo2FA;
		_loginView.switchToRegisterButton.OnFullClicked += SwitchToRegister;
		UIObjects.Add(_loginView);

		// Register view
		_registerView = new RegisterViewControl();
		_registerView.OnEmailNotConfirmed += SwitchTo2FA;
		_registerView.switchToLoginButton.OnFullClicked += SwitchToLogin;
		_registerView.Visible = false;
		UIObjects.Add(_registerView);

		// 2FA view
		_twoFAView = new TwoFAViewControl();
		_twoFAView.OnSuccess += SwitchToLogin;
		_twoFAView.switchToLoginButton.OnFullClicked += SwitchToLogin;
		_twoFAView.Visible = false;
		UIObjects.Add(_twoFAView);

		// 2FA view
		_notARobotView = new NotARobotViewControl();
		_notARobotView.Visible = false;
		_notARobotView.OnSuccess += OnNotARobotSuccess;
		_notARobotView.OnFail += () => { SwitchToLogin(); _loginView.resultLabel.Text = "Bad not a robot, please try again"; };
		UIObjects.Add(_notARobotView);
	}

	private void OnNotARobotSuccess()
	{
		new MainMenuScene().LoadScene();
	}

	private void SwitchToNotARobot()
	{
		_notARobotView.Reset();
		_notARobotView.Visible = true;
	}

	private void SwitchToLogin()
	{
		_loginView.Visible = true;
	}

	private void SwitchToRegister()
	{
		_registerView.Visible = true;
	}

	private void SwitchTo2FA()
	{
		_twoFAView.Visible = true;
	}
}