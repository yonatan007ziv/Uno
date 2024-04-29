using GameEngine.Components.UIComponents;
using System.Numerics;
using Uno.Core.Utilities.CommunicationProtocols;

namespace Uno.Client.GameComponents.Elements;

/// <summary>
/// One of the local player's cards, functioning with on click event
/// </summary>
internal class CardViewControl : UIButton
{
	public Card Card { get; }

	public CardViewControl(Card card, Action<Card> clickedOnCard)
	{
		// Card aspect ratio
		Transform.Scale = new Vector3(0.388f / 5, 0.562f, 1);
		Meshes.Add(new GameEngine.Core.Components.MeshData("UIRect.obj", $"{card}.mat"));
		Card = card;
		OnFullClicked += () => clickedOnCard(card);
		Enabled = false;
	}

	/// <summary>
	/// Enables the clicking mechanism for the card
	/// </summary>
	public void EnableCard()
	{
		Enabled = true;
	}

	/// <summary>
	/// Disables the clicking mechanism for the card
	/// </summary>
	public void DisableCard()
	{
		Enabled = false;
	}
}