using System.Net.Sockets;
using System.Text;
using Uno.Core.Utilities.Encryption;

namespace Uno.Core.Utilities.Networking;

public abstract class BaseTcpHandler
{
	protected const string EncryptionTestWord = "Success";

	// Encryption handler
	protected EncryptionHandler encryptionHandler;

	// Underlying socket
	public TcpClient Socket { get; private set; }

	protected TaskCompletionSource encryptionTask;

	public event Action? OnDisconnected;

	public BaseTcpHandler(TcpClient client)
	{
		Socket = client;
		encryptionHandler = new EncryptionHandler();
		encryptionTask = new TaskCompletionSource();
	}

	protected abstract Task<bool> EstablishEncryption();

	public async Task InitializeEncryption()
	{
		if (!await EstablishEncryption())
			throw new Exception();

		encryptionTask.SetResult();
	}

	#region Read write message
	public async Task WriteMessage(string message)
	{
		await encryptionTask.Task;

		// Get bytes from the message
		byte[] decryptedWriteBuffer = Encoding.UTF8.GetBytes(message);

		// Encrypt the bytes
		byte[] writeBuffer = encryptionHandler.EncryptAes(decryptedWriteBuffer);

		// Write them to the endpoint
		await WriteBytes(writeBuffer);
		return;
	}
	public async Task<string> ReadMessage()
	{
		await encryptionTask.Task;

		byte[] encryptedReadBuffer = await ReadBytes();
		byte[] readBuffer = encryptionHandler.DecryptAes(encryptedReadBuffer);
		string message = Encoding.UTF8.GetString(readBuffer);

		return message;
	}
	#endregion

	#region Read write bytes
	protected async Task WriteBytes(byte[] writeBuffer)
	{
		// Prefixes 4 Bytes Indicating Message Length
		byte[] length = BitConverter.GetBytes(writeBuffer.Length);
		byte[] prefixedBuffer = new byte[writeBuffer.Length + sizeof(int)];

		Array.Copy(length, 0, prefixedBuffer, 0, sizeof(int));
		Array.Copy(writeBuffer, 0, prefixedBuffer, sizeof(int), writeBuffer.Length);

		await Socket.GetStream().WriteAsync(prefixedBuffer);
	}
	protected async Task<byte[]> ReadBytes()
	{
		// Reads 4 Bytes Indicating Message Length
		byte[] lengthBuffer = new byte[4];
		await Socket.GetStream().ReadAsync(lengthBuffer);

		int length = BitConverter.ToInt32(lengthBuffer);
		byte[] readBuffer = new byte[length];
		int bytesRead = await Socket.GetStream().ReadAsync(readBuffer);

		if (bytesRead == 0)
			throw new Exception();

		return readBuffer;
	}
	#endregion
}