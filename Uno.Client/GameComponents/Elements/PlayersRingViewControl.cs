using GameEngine.Core.Components.Objects;
using System.Numerics;
using Uno.Core.Utilities.CommunicationProtocols;
using Uno.Core.Utilities.Models;

namespace Uno.Client.GameComponents.Elements;

/// <summary>
/// All of the players' visualization inside a ring
/// </summary>
internal class PlayersRingViewControl : UIObject
{
	public LocalCardsBeltViewControl playerCards;
	private Dictionary<int, EnemyPlayerView> enemyPlayers = new Dictionary<int, EnemyPlayerView>();

	public PlayersRingViewControl(Action<Card> clickedOnCard)
	{
		playerCards = new LocalCardsBeltViewControl(clickedOnCard);
		playerCards.Transform.Scale = new Vector3(0.65f, 0.25f, 1);
		Children.Add(playerCards);

		ArrangeVisuals();
	}

	/// <summary>
	/// Arrange all players in a circular pattern
	/// </summary>
	public void ArrangeVisuals()
	{
		int playerCount = 1 + enemyPlayers.Count;

		// Trigonometry to make a circular pattern
		float radius = 0.75f;
		for (int i = 0; i < playerCount; i++)
		{
			// Formula to start at the bottom and move in a counter-clockwise direction in even spacing
			float t = 2 * i * MathF.PI / playerCount - MathF.PI / 2;

			float x = MathF.Cos(t) * radius;
			float y = MathF.Sin(t) * radius;

			if (i == 0)
				playerCards.Transform.Position = new Vector3(x, y, 1);
			else
			{
				enemyPlayers.ElementAt(i - 1).Value.Transform.Position = new Vector3(x, y, 1);
				enemyPlayers.ElementAt(i - 1).Value.Transform.Scale = new Vector3(0.2f, 0.2f, 1);
			}
		}
	}

	/// <summary>
	/// Adds an enemy view
	/// </summary>
	/// <param name="enemy"> The enemy to add </param>
	public void AddEnemy(PlayerGameDataModel enemy)
	{
		enemyPlayers.Add(enemy.Id, new EnemyPlayerView(enemy));
		Children.Add(enemyPlayers[enemy.Id]);

		ArrangeVisuals();
	}

	/// <summary>
	/// Removes an enemy view
	/// </summary>
	/// <param name="enemy"> The enemy to remove </param>
	public void RemoveEnemy(PlayerGameDataModel enemy)
	{
		Children.Remove(enemyPlayers[enemy.Id]);
		enemyPlayers.Remove(enemy.Id);

		ArrangeVisuals();
	}

	/// <summary>
	/// Adds a card to an enemy
	/// </summary>
	/// <param name="enemy"> The enemy to add a card to </param>
	public void AddEnemyCard(PlayerGameDataModel enemy)
	{
		enemyPlayers[enemy.Id].AddCardCover();
		ArrangeVisuals();
	}

	/// <summary>
	/// Removes a card from an enemy
	/// </summary>
	/// <param name="enemy"> The enemy to remove a card from </param>
	public void RemoveEnemyCard(PlayerGameDataModel enemy)
	{
		enemyPlayers[enemy.Id].RemoveCardCover();
		ArrangeVisuals();
	}
}