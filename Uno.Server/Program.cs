using System.Net;
using Uno.Core.Utilities;
using Uno.Server.Components.Networking;
using Uno.Server.Components.Networking.ClientHandlers;

namespace Uno.Server;

internal class Program
{
	/// <summary>
	/// Entry point: starts both the login and gameplay servers
	/// </summary>
	public static async Task Main()
	{
		Task loginServer = new TcpServer<AuthenticationProcessClientHandler>(IPAddress.Parse(DevConstants.LoginRegisterServerAddress), DevConstants.LoginRegisterServerPort).Start();
		Task gameplayServer = new TcpServer<AuthenticatedSessionClientHandler>(IPAddress.Parse(DevConstants.GameplayServerAddress), DevConstants.GameplayServerPort).Start();

		// Wait for both servers to execute to completion
		await Task.WhenAll(loginServer, gameplayServer);
	}
}