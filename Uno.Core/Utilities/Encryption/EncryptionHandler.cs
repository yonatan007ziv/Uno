using System.Security.Cryptography;

namespace Uno.Core.Utilities.Encryption;

/// <summary>
/// Handles everything relates to the underlying encryption protocols
/// </summary>
public class EncryptionHandler
{
	private readonly RSA rsa = RSA.Create();
	private readonly Aes aes = Aes.Create();

	public EncryptionHandler()
	{
		aes.Padding = PaddingMode.PKCS7;
		aes.Mode = CipherMode.CBC;
	}

	/// <summary>
	/// Encrypts a buffer using Aes
	/// </summary>
	/// <param name="buffer"> The buffer to encrypt </param>
	/// <returns> The encrypted buffer </returns>
	public byte[] EncryptAes(byte[] buffer)
	{
		try
		{
			return aes.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length);
		}
		catch { throw new CryptographicException(); }
	}

	/// <summary>
	/// Encrypts a buffer using Rsa
	/// </summary>
	/// <param name="buffer"> The buffer to encrypt </param>
	/// <returns> The encrypted buffer </returns>
	public byte[] EncryptRsa(byte[] buffer)
	{
		try
		{
			return rsa.Encrypt(buffer, RSAEncryptionPadding.OaepSHA256);
		}
		catch { throw new CryptographicException(); }
	}

	/// <summary>
	/// Decrypts a buffer using Aes
	/// </summary>
	/// <param name="buffer"> The buffer to decrypt </param>
	/// <returns> The decrypted buffer </returns>
	public byte[] DecryptAes(byte[] buffer)
	{
		try
		{
			return aes.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length);
		}
		catch { throw new CryptographicException(); }
	}

	/// <summary>
	/// Decrypts a buffer using Rsa
	/// </summary>
	/// <param name="buffer"> The buffer to decrypt </param>
	/// <returns> The decrypted buffer </returns>
	public byte[] DecryptRsa(byte[] buffer)
	{
		try
		{
			return rsa.Decrypt(buffer, RSAEncryptionPadding.OaepSHA256);
		}
		catch { throw new CryptographicException(); }
	}

	/// <summary>
	/// Imports Rsa settings
	/// </summary>
	/// <param name="rsaPublicKey"> The public key to import </param>
	public void ImportRsa(byte[] rsaPublicKey)
	{
		try
		{
			rsa.ImportRSAPublicKey(rsaPublicKey, out _);
		}
		catch { throw new CryptographicException(); }
	}

	/// <summary>
	/// Exports Rsa settings
	/// </summary>
	/// <returns> The Rsa's public key </returns>
	public byte[] ExportRsa()
	{
		try
		{
			return rsa.ExportRSAPublicKey();
		}
		catch { throw new CryptographicException(); }
	}

	/// <summary>
	/// Imports Aes private key
	/// </summary>
	/// <param name="aesPrivateKey"> Aes private key to import </param>
	public void ImportAesPrivateKey(byte[] aesPrivateKey)
	{
		aes.Key = aesPrivateKey;
	}

	/// <summary>
	/// Imports Aes iv
	/// </summary>
	/// <param name="aesIv"> Aes iv to import </param>
	public void ImportAesIv(byte[] aesIv)
	{
		aes.IV = aesIv;
	}

	/// <summary>
	/// Exports Aes private key
	/// </summary>
	/// <returns> The Aes's private key </returns>
	public byte[] ExportAesPrivateKey()
	{
		return aes.Key;
	}

	/// <summary>
	/// Exports Aes iv
	/// </summary>
	/// <returns> The Aes's iv </returns>
	public byte[] ExportAesIv()
	{
		return aes.IV;
	}
}