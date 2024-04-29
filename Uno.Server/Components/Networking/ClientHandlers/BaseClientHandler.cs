namespace Uno.Server.Components.Networking.ClientHandlers;

/// <summary>
/// The base abstract client handler
/// </summary>
internal abstract class BaseClientHandler
{
	public TcpClientHandler TcpClientHandler { get; set; } = null!;
	public event Action? OnDisconnect;

	protected void Disconnect()
	{
		OnDisconnect?.Invoke();
	}

	public abstract void StartRead();
}