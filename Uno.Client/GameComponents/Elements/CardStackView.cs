using GameEngine.Core.Components.Objects;
using System.Drawing;
using System.Numerics;
using Uno.Core.Utilities.CommunicationProtocols;

namespace Uno.Client.GameComponents.Elements;

/// <summary>
/// A generic non-functioning view of a card used to visualize the cards stack
/// </summary>
internal class CardStackView : UIObject
{
	public CardStackView(Card card)
	{
		// Card aspect ratio
		Transform.Scale = new Vector3(0.388f, 0.662f, 1) / 3.5f;

		// Random rotation
		Transform.Rotation = new Vector3(0, 0, new Random().Next(-45, 46));

		Meshes.Add(new GameEngine.Core.Components.MeshData("UIRect.obj", $"{card}.mat"));
	}

	public CardStackView(Color color)
	{
		// Card aspect ratio
		Transform.Scale = new Vector3(0.388f, 0.662f, 1) / 3.5f;

		// Random rotation
		Transform.Rotation = new Vector3(0, 0, new Random().Next(-45, 46));

		Meshes.Add(new GameEngine.Core.Components.MeshData("UIRect.obj", $"{color.ToKnownColor()}.mat"));
	}
}