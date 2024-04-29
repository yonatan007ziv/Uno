using GameEngine.Components.UIComponents;
using GameEngine.Core.Components.Objects;
using System.Numerics;
using Uno.Core.Utilities.Models;

namespace Uno.Client.GameComponents.Elements;

/// <summary>
/// Enemy player view
/// </summary>
internal class EnemyPlayerView : UIObject
{
	private UILabel nameLabel;
	private List<CardCoverView> covers = new List<CardCoverView>();

	public EnemyPlayerView(PlayerGameDataModel player)
	{
		nameLabel = new UILabel();
		nameLabel.Transform.Scale = new Vector3(1, 0.2f, 1);
		nameLabel.Transform.Position = new Vector3(0, 0.8f, 1);
		nameLabel.Text = player.Name;
		Children.Add(nameLabel);
	}

	/// <summary>
	/// Adds a card cover
	/// </summary>
	public void AddCardCover()
	{
		covers.Add(new CardCoverView());
		Children.Add(covers.Last());
		RearrangeCards();
	}


	/// <summary>
	/// Removes a card cover
	/// </summary>
	public void RemoveCardCover()
	{
		if (covers.Count > 0)
		{
			Children.Remove(covers.First());
			covers.RemoveAt(0);
		}
		RearrangeCards();
	}

	/// <summary>
	/// Rearranges the cards' visual
	/// </summary>
	private void RearrangeCards()
	{
		float startX = -1 + 0.2f, yOffset = 0;
		int untilLoweringY = 10;
		for (int i = 0; i < covers.Count; i++)
		{
			if (i != 0 && i % untilLoweringY == 0)
			{
				yOffset -= 0.2f;
			}
			float xPos = startX + (i % untilLoweringY * 0.3f);
			covers[i].Transform.Position = new Vector3(xPos, yOffset, 0);
		}
	}
}