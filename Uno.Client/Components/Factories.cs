using GameEngine.Core.SharedServices.Interfaces;
using Uno.Client.Components.Networking;
using Uno.Core.Utilities.Loggers;

namespace Uno.Client.Components;

/// <summary>
/// Hold sttaic references to relevant factories
/// </summary>
internal static class Factories
{
	public static IFactory<TcpClientHandler> ClientFactory { get; }

	static Factories()
	{
		ClientFactory = new TcpClientFactory(new ConsoleLogger());
	}
}