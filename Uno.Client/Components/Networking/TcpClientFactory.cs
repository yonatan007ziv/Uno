using GameEngine.Core.SharedServices.Interfaces;
using System.Net.Sockets;
using Uno.Core.Utilities.Loggers;

namespace Uno.Client.Components.Networking;

/// <summary>
/// A factory for TcpClientHandlers
/// </summary>
internal class TcpClientFactory : IFactory<TcpClientHandler>
{
	private readonly ConsoleLogger consoleLogger;

	public TcpClientFactory(ConsoleLogger consoleLogger)
	{
		this.consoleLogger = consoleLogger;
	}

	/// <summary>
	/// Creats a new TcpClientHandler instance
	/// </summary>
	/// <param name="result"> The created out TcpClientHandler </param>
	/// <returns> True if succeeded, false otherwise </returns>
	public bool Create(out TcpClientHandler result)
	{
		result = new TcpClientHandler(new TcpClient(), consoleLogger);
		return true;
	}
}