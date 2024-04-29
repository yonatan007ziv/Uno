using System.Net;

namespace Uno.Server.Components.Networking;

/// <summary>
/// Checks DoS status
/// </summary>
internal static class DoSChecker
{
	// 25 requests per 1 minutes to the user
	private const int NumOfAllowedRequests = 25;
	private static readonly TimeSpan TimeUntilForgetsRequests = TimeSpan.FromMinutes(1);
	private static readonly Dictionary<string, int> clientTracker = new Dictionary<string, int>();

	/// <summary>
	/// Checks if the current state is healthy
	/// </summary>
	/// <param name="ip"> The tracked ip </param>
	/// <returns> True if healthy, false otherwise </returns>
	public static bool CheckHealthy(IPAddress ip)
	{
		string ipStr = ip.ToString();
		if (clientTracker.TryGetValue(ipStr, out int value))
		{
			clientTracker[ipStr] = ++value;

			if (value > NumOfAllowedRequests)
			{
				ReduceAfterTime(ipStr);
				return false;
			}
		}
		else
			clientTracker[ipStr] = 1;

		ReduceAfterTime(ipStr);
		return true;
	}

	/// <summary>
	/// Reduces the number of requests in the handler's counter by 1 after a certain amount of time
	/// </summary>
	/// <param name="ip"> The ip to reduce the counter of </param>
	private static async void ReduceAfterTime(string ip)
	{
		await Task.Delay(TimeUntilForgetsRequests);
		clientTracker[ip]--;
	}
}