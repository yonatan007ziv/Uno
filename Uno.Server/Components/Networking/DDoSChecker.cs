namespace Uno.Server.Components.Networking;

/// <summary>
/// Checks DDoS status
/// </summary>
internal static class DDoSChecker
{
	// 50 requests per minute as a whole to the server
	private const int NumOfAllowedRequests = 50;
	private static readonly TimeSpan TimeUntilForgetsRequests = TimeSpan.FromMinutes(1);
	private static int currentRequestCount;

	/// <summary>
	/// Checks if the current state is healthy
	/// </summary>
	/// <returns> True if healthy, false otherwise </returns>
	public static bool CheckHealthy()
	{
		currentRequestCount++;
		ReduceAfterTime();

		if (currentRequestCount > NumOfAllowedRequests)
			return false;
		return true;
	}

	/// <summary>
	/// Reduces the number of requests by 1 after a certain time
	/// </summary>
	private static async void ReduceAfterTime()
	{
		await Task.Delay(TimeUntilForgetsRequests);
		currentRequestCount--;
	}
}