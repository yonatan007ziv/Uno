using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;
using Uno.Core.Utilities.Networking;

namespace Uno.Server.Components.Networking;

/// <summary>
/// Handles a TcpClient
/// </summary>
internal class TcpClientHandler : BaseTcpHandler
{
	private readonly ILogger logger;
	public TcpClientHandler(TcpClient client, ILogger logger)
		: base(client)
	{
		this.logger = logger;
	}

	/// <summary>
	/// Writes a message
	/// </summary>
	/// <param name="message"> The message to write </param>
	/// <returns> True if the message was written successfully, false otherwise </returns>
	public async new Task<bool> WriteMessage(string message)
	{
		try
		{
			await base.WriteMessage(message);
			return true;
		}
		catch { Disconnect(); return false; }
	}

	/// <summary>
	/// Reads a message
	/// </summary>
	/// <returns> The read message </returns>
	public async new Task<string?> ReadMessage()
	{
		try
		{
			return await base.ReadMessage();
		}
		catch { Disconnect(); return null; }
	}

	/// <summary>
	/// Establishes the encryption using a well defined protocol
	/// </summary>
	/// <returns> True if the encryption was successfully established, false otherwise </returns>
	protected override async Task<bool> EstablishEncryption()
	{
		try
		{
			// Import Rsa Details
			byte[] rsaPublicKey = await ReadBytes();
			encryptionHandler.ImportRsa(rsaPublicKey);

			// Send Aes Details
			byte[] aesPrivateKey = encryptionHandler.ExportAesPrivateKey();
			byte[] encryptedRsaPrivateKey = encryptionHandler.EncryptRsa(aesPrivateKey);
			_ = WriteBytes(encryptedRsaPrivateKey);
			byte[] aesIv = encryptionHandler.ExportAesIv();
			byte[] encryptedRsaIv = encryptionHandler.EncryptRsa(aesIv);
			_ = WriteBytes(encryptedRsaIv);

			// Test Encryption: Send
			string msgTest = EncryptionTestWord;
			byte[] decryptedTest = Encoding.UTF8.GetBytes(msgTest);
			byte[] encryptedTest = encryptionHandler.EncryptAes(decryptedTest);
			_ = WriteBytes(encryptedTest);

			// Test Encryption: Receive
			encryptedTest = await ReadBytes();
			decryptedTest = encryptionHandler.DecryptAes(encryptedTest);
			msgTest = Encoding.UTF8.GetString(decryptedTest);

			if (msgTest != EncryptionTestWord)
				throw new Exception($"Encryption error, Expected {EncryptionTestWord}, got {msgTest} ");
		}
		catch (Exception ex)
		{
			logger.LogError("Failed Establishing Encryption: {exceptionMessage}", ex.Message);
			return false;
		}

		logger.LogInformation("End-to-end encryption was Successful");
		return true;
	}

	/// <summary>
	/// Disconnects from the connected endpoint
	/// </summary>
	public void Disconnect()
	{
		Socket.Close();
	}
}