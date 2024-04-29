using GameEngine.Components.UIComponents;
using GameEngine.Core.Components.Objects;
using System.Drawing;
using System.Numerics;
using Uno.Core.Utilities.CommunicationProtocols;

namespace Uno.Client.GameComponents.Elements;

/// <summary>
/// The cards belt view for the local player
/// </summary>
internal class LocalCardsBeltViewControl : UIObject
{
	private const int cardsPerPage = 10;

	private List<(Card cardType, CardViewControl cardViewControl)> cards = new List<(Card, CardViewControl)>();
	private readonly Action<Card> clickedOnCard;

	private UIButton arrowScrollRight;
	private UIButton arrowScrollLeft;

	private int scrollIndex = 0;

	public LocalCardsBeltViewControl(Action<Card> clickedOnCard)
	{
		this.clickedOnCard = clickedOnCard;

		arrowScrollRight = new UIButton();
		arrowScrollRight.Transform.Scale = new Vector3(0.1f, 0.1f, 1);
		arrowScrollRight.Transform.Position = new Vector3(0.9f, 0, 0);
		arrowScrollRight.Text = "->";
		arrowScrollRight.TextColor = Color.White;
		arrowScrollRight.Visible = false;
		arrowScrollRight.OnFullClicked += ScrollRight;
		Children.Add(arrowScrollRight);

		arrowScrollLeft = new UIButton();
		arrowScrollLeft.Transform.Scale = new Vector3(0.1f, 0.1f, 1);
		arrowScrollLeft.Transform.Position = new Vector3(-0.9f, 0, 0);
		arrowScrollLeft.Text = "<-";
		arrowScrollLeft.TextColor = Color.White;
		arrowScrollLeft.Visible = false;
		arrowScrollLeft.OnFullClicked += ScrollLeft;
		Children.Add(arrowScrollLeft);
	}

	/// <summary>
	/// Checks if should show scroll arrows and acts accordingly
	/// </summary>
	private void HideScrollArrowsOnLimits()
	{
		int maxScrollIndex = (int)MathF.Ceiling((float)cards.Count / cardsPerPage);
		if (scrollIndex == maxScrollIndex - 1)
			arrowScrollRight.Visible = false;
		else if (scrollIndex == 0)
			arrowScrollLeft.Visible = false;
		else if (scrollIndex > maxScrollIndex - 1)
			scrollIndex--;
		else
		{
			arrowScrollRight.Visible = true;
			arrowScrollLeft.Visible = true;
		}
	}

	/// <summary>
	/// Scroll one index right
	/// </summary>
	private void ScrollRight()
	{
		if (scrollIndex + 1 < MathF.Ceiling((float)cards.Count / cardsPerPage))
		{
			scrollIndex++;
			RearrangeCards();
		}

		HideScrollArrowsOnLimits();
	}

	/// <summary>
	/// Scroll one index left
	/// </summary>
	private void ScrollLeft()
	{
		if (scrollIndex != 0)
		{
			scrollIndex--;
			RearrangeCards();
		}

		HideScrollArrowsOnLimits();
	}

	/// <summary>
	/// When clicked on a card, call the given clickedOnCard delegate, remove it from the cards and then rearrange them
	/// </summary>
	/// <param name="clicked"></param>
	private void OnClickedOnCard(Card clicked)
	{
		clickedOnCard(clicked);
		RemoveCard(clicked);
		RearrangeCards();
	}

	/// <summary>
	/// Adds a new card
	/// </summary>
	/// <param name="toAdd"> The card to add </param>
	public void AddCard(Card toAdd)
	{
		CardViewControl card = new CardViewControl(toAdd, OnClickedOnCard);
		cards.Add((toAdd, card));
		Children.Add(card);
		RearrangeCards();
	}

	/// <summary>
	/// Removes a card
	/// </summary>
	/// <param name="clicked"> The card to remove </param>
	public void RemoveCard(Card clicked)
	{
		(Card cardType, CardViewControl cardViewControl) toRemove = cards.Find(card => card.cardType == clicked);
		Children.Remove(toRemove.cardViewControl);
		cards.Remove(toRemove);
		RearrangeCards();
	}

	/// <summary>
	/// Enables all cards that can be placed on top of the current cards stack
	/// </summary>
	/// <param name="last"> The last card on the cards stack </param>
	/// <param name="didSwitchColor"> The last operation was switching colors </param>
	/// <param name="switchedToColor"> The color that was switched to </param>
	public void EnableAccordingToLast(Card last, bool didSwitchColor, Color switchedToColor)
	{
		for (int i = 0; i < cards.Count; i++)
			if (AppropriateToPlaceOn(cards[i].cardType, last, didSwitchColor, switchedToColor))
				cards[i].cardViewControl.EnableCard();
	}

	/// <summary>
	/// Disable all cards
	/// </summary>
	public void DisableAll()
	{
		for (int i = 0; i < cards.Count; i++)
			cards[i].cardViewControl.DisableCard();
	}

	/// <summary>
	/// Rearrange the cards' visual
	/// </summary>
	public void RearrangeCards()
	{
		float xPos = arrowScrollLeft.Transform.Position.X + arrowScrollLeft.Transform.Scale.X + 0.1f;
		for (int i = 0; i < cards.Count; i++)
			cards[i].cardViewControl.Visible = false;
		for (int i = 0; i < cardsPerPage && i < cardsPerPage; i++)
		{
			if (cards.Count <= i + scrollIndex * cardsPerPage)
				continue;

			cards[i + scrollIndex * cardsPerPage].cardViewControl.Visible = true;
			cards[i + scrollIndex * cardsPerPage].cardViewControl.Transform.Position = new Vector3(xPos, 0, 0);
			xPos += cards[i + scrollIndex * cardsPerPage].cardViewControl.Transform.Scale.X * 2;
		}

		if (cards.Count > 10)
		{
			arrowScrollRight.Visible = true;
			arrowScrollLeft.Visible = true;
		}
		else
		{
			arrowScrollRight.Visible = false;
			arrowScrollLeft.Visible = false;
		}

		HideScrollArrowsOnLimits();
	}

	/// <summary>
	/// Checks if a card can be placed on top of a card
	/// </summary>
	/// <param name="card1"> The card to place </param>
	/// <param name="card2"> The current card to place on </param>
	/// <param name="didSwitchColor"> The last operation was switching colors </param>
	/// <param name="switchedToColor"> The color that was switched to </param>
	/// <returns></returns>
	private bool AppropriateToPlaceOn(Card card1, Card card2, bool didSwitchColor, Color switchedToColor)
	{
		if (card1 == Card.Wild || card1 == Card.Wild_Draw)
			return true;

		if (didSwitchColor)
			return switchedToColor.ToKnownColor().ToString() == card1.ToString().Split('_')[0];

		if (card2 == Card.Wild_Draw)
			return true;

		string color1 = card1.ToString().Split('_')[0];
		string color2 = card2.ToString().Split('_')[0];
		string value1 = card1.ToString().Split('_')[1];
		string value2 = card2.ToString().Split('_')[1];

		return color1 == color2 || value1 == value2;
	}
}