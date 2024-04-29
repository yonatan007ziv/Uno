using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using Uno.Core.Utilities.Loggers;
using Uno.Server.Components.Networking.ClientHandlers;

namespace Uno.Server.Components.Networking;

/// <summary>
/// Constructs a tcp server with a generic client handler
/// </summary>
/// <typeparam name="TClientHandler"> The generic client handler </typeparam>
internal class TcpServer<TClientHandler> where TClientHandler : BaseClientHandler, new()
{
	private readonly TcpListener listener;
	private readonly Dictionary<TcpClientHandler, BaseClientHandler> clients = new Dictionary<TcpClientHandler, BaseClientHandler>();
	private readonly TimeSpan DDosFreezeTime = TimeSpan.FromSeconds(5);

	private bool ddosProtection;

	public bool Run { get; set; }
	public ObservableCollection<TcpClientHandler> Clients { get; }

	public TcpServer(System.Net.IPAddress address, int port)
	{
		listener = new TcpListener(address, port);
		Clients = new ObservableCollection<TcpClientHandler>();
		Clients.CollectionChanged += ClientJoinedLeft;
	}

	/// <summary>
	/// Called when a new client has joined or left
	/// </summary>
	private void ClientJoinedLeft(object? s, NotifyCollectionChangedEventArgs e)
	{
		// Client was added
		if (e.Action == NotifyCollectionChangedAction.Add)
			foreach (object clientHandler in e.NewItems!)
			{
				if (clientHandler is TcpClientHandler tcpClientHandler)
					OnClientJoin(tcpClientHandler);
			}
		// Client was Removed
		else if (e.Action == NotifyCollectionChangedAction.Remove)
			foreach (object clientHandler in e.OldItems!)
			{
				if (clientHandler is TcpClientHandler tcpClientHandler)
					OnClientLeave(tcpClientHandler);
			}
	}

	/// <summary>
	/// Called when a client joins
	/// </summary>
	private void OnClientJoin(TcpClientHandler tcpClientHandler)
	{
		TClientHandler client = new TClientHandler();

		client.OnDisconnect += () => Clients.Remove(tcpClientHandler);

		client.TcpClientHandler = tcpClientHandler;

		client.StartRead();

		clients.Add(tcpClientHandler, client);
	}

	/// <summary>
	/// Called when a client leaves
	/// </summary>
	private void OnClientLeave(TcpClientHandler tcpClientHandler)
	{
		clients.Remove(tcpClientHandler);
	}

	/// <summary>
	/// Starts the server
	/// </summary>
	/// <returns> A task representing the server's state </returns>
	public async Task Start()
	{
		Run = true;

		listener.Start();
		await AcceptClients();
	}

	/// <summary>
	/// A loop for accepting new clients
	/// </summary>
	/// <returns> A task representing the server's state </returns>
	private async Task AcceptClients()
	{
		while (Run)
		{
			TcpClient tcpSocket = await listener.AcceptTcpClientAsync();
			IPAddress ip = ((IPEndPoint)tcpSocket.Client.RemoteEndPoint!).Address;
			TcpClientHandler tcpClientHandler = new TcpClientHandler(tcpSocket, new ConsoleLogger());

			if (!DoSChecker.CheckHealthy(ip))
			{
				await Console.Out.WriteLineAsync($"Blocking connection from [{ip}] due to active DoS protection...");
				tcpClientHandler.Disconnect();
				continue;
			}

			if (ddosProtection || !DDoSChecker.CheckHealthy())
			{
				await Console.Out.WriteLineAsync($"Blocking connection from [{ip}] due to active DDoS protection...");
				tcpClientHandler.Disconnect();
				DDoSFreezeServerForTime();
				continue;
			}

			_ = tcpClientHandler.InitializeEncryption();
			Clients.Add(tcpClientHandler);
		}
	}

	/// <summary>
	/// Freezes the server to fight against DDoS attempts
	/// Flushes out requests after freeze finished
	/// </summary>
	/// <returns> A task representing a delay </returns>
	private async void DDoSFreezeServerForTime()
	{
		ddosProtection = true;
		await Task.Delay(DDosFreezeTime);
		ddosProtection = false;
	}
}