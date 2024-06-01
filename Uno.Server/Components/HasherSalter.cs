using System.Security.Cryptography;
using System.Text;

namespace Uno.Server.Components;

/// <summary>
/// Used for salting and hashing passwords and sensitive data in the database
/// </summary>
internal static class HasherSalter
{
	private static readonly MD5 hasher = MD5.Create();
	private static readonly Random random = new Random();
	private static readonly int saltLength;

	static HasherSalter()
	{
		saltLength = hasher.HashSize;
	}

	/// <summary>
	/// Hashes and salts a password
	/// </summary>
	/// <param name="password"> The password to hash and salt </param>
	/// <param name="salt"> The salt </param>
	/// <param name="saltedHash"> The return password salted hash </param>
	public static void HashSaltPassword(string password, byte[] salt, out byte[] saltedHash)
	{
		byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
		byte[] hashedPasswordArray = HashArray(passwordBytes);

		saltedHash = SaltHash(hashedPasswordArray, salt);
	}

	/// <summary>
	/// Hashes the given password with a random salt
	/// </summary>
	/// <param name="password"> The password to hash </param>
	/// <param name="hash"> The out returned hash </param>
	/// <param name="salt"> The out returned salt used </param>
	public static void HashSaltPasswordRandomSalt(string password, out byte[] hash, out byte[] salt)
	{
		byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
		byte[] hashedPasswordArray = HashArray(passwordBytes);

		salt = RandomSalt();
		hash = SaltHash(hashedPasswordArray, salt);
	}

	/// <summary>
	/// Generates a random salt
	/// </summary>
	/// <returns> The random salt </returns>
	private static byte[] RandomSalt()
	{
		const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		StringBuilder res = new StringBuilder();

		for (int i = 0; i < saltLength; i++)
			res.Append(valid[random.Next(valid.Length)]);

		return Encoding.ASCII.GetBytes(res.ToString());
	}

	/// <summary>
	/// Hashes a byte array in a one-way fashion
	/// </summary>
	/// <param name="arr"> The array to hash </param>
	/// <returns> The hashed array </returns>
	public static byte[] HashArray(byte[] arr)
	{
		return hasher.ComputeHash(arr);
	}

	/// <summary>
	/// Salts an hash
	/// </summary>
	/// <param name="hash"> The hash to salt </param>
	/// <param name="salt"> The salt used to salt the hash </param>
	/// <returns> The hashed and salted hash </returns>
	private static byte[] SaltHash(byte[] hash, byte[] salt)
	{
		HashAlgorithm algorithm = SHA256.Create();

		byte[] hashWithSaltBytes = new byte[hash.Length + salt.Length];

		for (int i = 0; i < hash.Length; i++)
			hashWithSaltBytes[i] = hash[i];
		for (int i = 0; i < salt.Length; i++)
			hashWithSaltBytes[hash.Length + i] = salt[i];

		return algorithm.ComputeHash(hashWithSaltBytes);
	}
}