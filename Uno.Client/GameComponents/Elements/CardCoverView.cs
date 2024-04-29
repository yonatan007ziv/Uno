using GameEngine.Core.Components.Objects;

namespace Uno.Client.GameComponents.Elements;

/// <summary>
/// A view of the card cover used to hide enemy player's cards
/// </summary>
internal class CardCoverView : UIObject
{
	public CardCoverView()
	{
		Transform.Scale /= 3;
		Meshes.Add(new GameEngine.Core.Components.MeshData("UIRect.obj", "Cover.mat"));
	}
}