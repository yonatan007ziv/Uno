using GameEngine.Components.UIComponents;
using GameEngine.Core.Components.Objects;
using System.Drawing;
using System.Net;
using System.Numerics;
using Uno.Client.Components;
using Uno.Client.Components.Networking;
using Uno.Core.Utilities.CommunicationProtocols.LoginRegister;
using Uno.Core.Utilities.MessageConstructors;
using Uno.Core.Utilities.Networking;

namespace Uno.Client.GameComponents.Views.LoginRegisterViewControls;

/// <summary>
/// Not a robot test view control
/// </summary>
internal class NotARobotViewControl : UIObject
{
	private UIButton? confirmNotARobot;
	private NotARobotSquare[] squares = null!;
	private TcpClientHandler clientHandler = null!;

	public event Action? OnSuccess;
	public event Action? OnFail;

	/// <summary>
	/// Resets the view
	/// </summary>
	public void Reset()
	{
		UILabel title = new UILabel();
		title.Transform.Position = new Vector3(0, 0.75f, 0);
		title.Transform.Scale = new Vector3(1, 0.5f, 0);
		title.Text = "Not a Robot\nplease make all of the squares green\nAnd then confirm";
		title.TextColor = Color.White;
		Children.Add(title);

		if (confirmNotARobot is not null)
			confirmNotARobot.OnFullClicked -= ConfirmNotARobotButton;

		RequestNotARobotPuzzle();
	}

	/// <summary>
	/// Request a puzzle
	/// </summary>
	public async void RequestNotARobotPuzzle()
	{
		if (!Factories.ClientFactory.Create(out clientHandler)
			|| !await clientHandler.Connect(IPAddress.Parse(ServerAddresses.LoginRegisterServerAddress), ServerAddresses.LoginRegisterServerPort))
			return;

		if (!await clientHandler.WriteMessage(AuthenticationProcessMessageConstructor.ConstructNotARobotRequest()))
			return;

		string? responseStr = await clientHandler.ReadMessage();
		if (responseStr is null)
			return;

		if (AuthenticationProcessMessageConstructor.DeconstructNotARobotResponse(responseStr, out NotARobotResponse response, out bool[] squares) && response == NotARobotResponse.Squares)
			CreateSquares(squares);
	}

	/// <summary>
	/// Creates the squares based on the given boolean array
	/// </summary>
	/// <param name="predeterminedButtons"> The predetermined array of booleans </param>
	private void CreateSquares(bool[] predeterminedButtons)
	{
		Vector3 startPosition = new Vector3(-0.25f, 0.25f, 0);
		float horizontalJump = 0.25f;
		float verticalJump = -0.25f;

		Vector3 squareSize = new Vector3(0.1f, 0.1f, 0);
		Vector3 position = startPosition;
		squares = new NotARobotSquare[predeterminedButtons.Length];

		for (int i = 0; i < predeterminedButtons.Length; i++)
		{
			bool dropDown = i % 3 == 0 && i != 0;
			if (dropDown)
			{
				position += Vector3.UnitY * verticalJump;
				position = new Vector3(startPosition.X, position.Y, 0);
			}

			squares[i] = new NotARobotSquare(predeterminedButtons[i]);
			squares[i].Transform.Scale = squareSize;
			squares[i].Transform.Position = position;
			Children.Add(squares[i]);


			position += Vector3.UnitX * horizontalJump;
		}

		confirmNotARobot = new UIButton();
		confirmNotARobot.OnFullClicked += ConfirmNotARobotButton;
		confirmNotARobot.Transform.Scale /= 5;
		confirmNotARobot.Transform.Position = new Vector3(0, -0.75f, 0);
		confirmNotARobot.Text = "Confirm not a Robot";
		confirmNotARobot.TextColor = Color.White;
		Children.Add(confirmNotARobot);
	}

	/// <summary>
	/// Confirm not a robot puzzle
	/// </summary>
	private void ConfirmNotARobotButton()
	{
		confirmNotARobot!.Enabled = false;

		bool failed = false;
		for (int i = 0; i < squares.Length; i++)
			if (!squares[i].SquareEnabled)
				failed = true;

		if (!failed)
			OnSuccess?.Invoke();
		else
			OnFail?.Invoke();

		Visible = false;
		confirmNotARobot.Enabled = true;
		clientHandler.Disconnect();
	}
}