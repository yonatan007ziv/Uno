using GameEngine.Components.UIComponents;
using GameEngine.Core.Components;

namespace Uno.Client.GameComponents.Elements;

/// <summary>
/// Not a robot singular square
/// </summary>
internal class NotARobotSquareViewControl : UIButton
{
	public bool SquareEnabled { get; private set; }

	public NotARobotSquareViewControl(bool predeterminedSelected)
	{
		OnFullClicked += OnClick;

		SquareEnabled = predeterminedSelected;
		Meshes.Add(new MeshData("UIRect.obj", $"{(predeterminedSelected ? "Green" : "Red")}.mat"));
	}

	/// <summary>
	/// When clicking the button, flip the flag and color
	/// </summary>
	public void OnClick()
	{
		Meshes.Clear();

		SquareEnabled = !SquareEnabled;
		Meshes.Add(new MeshData("UIRect.obj", $"{(SquareEnabled ? "Green" : "Red")}.mat"));
	}
}