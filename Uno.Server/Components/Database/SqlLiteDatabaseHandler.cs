using Microsoft.Data.Sqlite;
using Uno.Core.Utilities;

namespace Uno.Server.Components.Database;

/// <summary>
/// Handles SQL connection and queries in a secured parameterized way
/// </summary>
internal static class SqlLiteDatabaseHandler
{
	private static readonly SqliteConnection conn = new SqliteConnection(DevConstants.DatabaseCredentials.ConnectionString);

	/// <summary>
	/// Checks whether a username exists in the table
	/// </summary>
	/// <param name="username"> Username to check if exists </param>
	/// <returns> True if the username exists in the table, false otherwise </returns>
	public static bool UsernameExists(string username)
	{
		string sql = @"SELECT COUNT(*) FROM [Users] WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		long count = (long)(cmd.ExecuteScalar() ?? 0);
		conn.Close();
		return count > 0;
	}

	/// <summary>
	/// Gets the 2fa code hash of the given username
	/// </summary>
	/// <param name="username"> The username to retrieve the 2fa hash of </param>
	/// <returns> The stored 2fa hash </returns>
	public static byte[] Get2FAHash(string username)
	{
		string sql = @"SELECT TwoFAHash FROM [Users] WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		byte[] arr = (byte[])(cmd.ExecuteScalar() ?? 0);
		conn.Close();
		return arr;
	}

	/// <summary>
	/// Sets the 2fa hash of the given username
	/// </summary>
	/// <param name="username"> The username to set the hash for </param>
	/// <param name="twoFAHash"> The 2fa hash to set </param>
	public static void Set2FAHash(string username, byte[] twoFAHash)
	{

		if (!UsernameExists(username))
			return;

		string sql = @"UPDATE [Users] SET TwoFAHash = @TwoFAHash WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);
		cmd.Parameters.AddWithValue("@TwoFAHash", twoFAHash);

		conn.Open();
		cmd.ExecuteNonQuery();
		conn.Close();
	}

	/// <summary>
	/// Sets the time of which the 2fa code was generated
	/// </summary>
	/// <param name="username"> The username linked to the 2fa code </param>
	/// <param name="lastTime"> The time of which the 2fa code was generated </param>
	public static void Set2FATime(string username, DateTime lastTime)
	{
		if (!UsernameExists(username))
			return;

		string twoFADateTime = lastTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

		string sql = @"UPDATE [Users] SET TwoFADateTime = @TwoFADateTime WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);
		cmd.Parameters.AddWithValue("@TwoFADateTime", twoFADateTime);

		conn.Open();
		cmd.ExecuteNonQuery();
		conn.Close();
	}

	/// <summary>
	/// Gets when the 2fa code linked to the username was generated
	/// </summary>
	/// <param name="username"> The linked username </param>
	/// <param name="dateTime"> the out parameter containing the time </param>
	/// <returns> Whether the operation was successful </returns>
	public static bool Get2FATime(string username, out DateTime dateTime)
	{
		string sql = @"SELECT TwoFADateTime FROM [Users] WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		string result = (string)(cmd.ExecuteScalar() ?? "");
		conn.Close();

		return DateTime.TryParseExact(result, "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime);
	}

	/// <summary>
	/// Checks if the email exists in the table
	/// </summary>
	/// <param name="email"> The email to check </param>
	/// <returns> True if the email exists, false otherwise </returns>
	public static bool EmailExists(string email)
	{
		string sql = @"SELECT COUNT(*) FROM [Users] WHERE Email = @Email";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Email", email);

		conn.Open();
		long count = (long)(cmd.ExecuteScalar() ?? 0);
		conn.Close();
		return count > 0;
	}

	/// <summary>
	/// Gets the email of the given username
	/// </summary>
	/// <param name="username"> Username to get the email of </param>
	/// <returns> The email linked to the given username </returns>
	public static string GetEmail(string username)
	{
		string sql = @"SELECT Email FROM [Users] WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		string count = (string)(cmd.ExecuteScalar() ?? 0);
		conn.Close();
		return count;
	}

	/// <summary>
	/// Gets the salted password hash of the given username
	/// </summary>
	/// <param name="username"> The username to get the salted password hash of </param>
	/// <returns> The salted password hash </returns>
	public static byte[] GetSaltedPasswordHash(string username)
	{
		string sql = @"SELECT PasswordHash FROM [Users] WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		byte[] result = (byte[])(cmd.ExecuteScalar() ?? "");
		conn.Close();
		return result;
	}

	/// <summary>
	/// Gets the password salt linked to the given username
	/// </summary>
	/// <param name="username"> The username to get the salt of </param>
	/// <returns> The password's salt </returns>
	public static byte[] GetPasswordSalt(string username)
	{
		string sql = @"SELECT PasswordSalt FROM [Users] WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		byte[] result = (byte[])(cmd.ExecuteScalar() ?? "");
		conn.Close();
		return result;
	}

	/// <summary>
	/// Checks whether the email linked to the given username is validated
	/// </summary>
	/// <param name="username"> The username to check the email of </param>
	/// <returns> True if it's validated, false otherwise </returns>
	public static bool GetEmailConfirmed(string username)
	{
		string sql = @"SELECT EmailConfirmed FROM [Users] WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		long result = (long)(cmd.ExecuteScalar() ?? 0);
		conn.Close();
		return result == 1;
	}

	/// <summary>
	/// Inserts a user
	/// </summary>
	/// <param name="username"> The username </param>
	/// <param name="email"> The email </param>
	/// <param name="saltedPasswordHash"> The salted password hash </param>
	/// <param name="passwordSalt"> The password salt </param>
	/// <param name="twoFAHash"> The 2fa hash </param>
	public static void InsertUser(string username, string email, byte[] saltedPasswordHash, byte[] passwordSalt, byte[] twoFAHash)
	{
		string sql = @"INSERT INTO [Users] (Username, PasswordHash, PasswordSalt, Email, EmailConfirmed, TwoFAHash, TwoFADateTime, WonTimes, LostTimes) VALUES (@Username, @SaltedPasswordHash, @PasswordSalt, @Email, 0, @TwoFAHash, @TwoFADateTime, 0, 0)";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);
		cmd.Parameters.AddWithValue("@SaltedPasswordHash", saltedPasswordHash);
		cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
		cmd.Parameters.AddWithValue("@Email", email);
		cmd.Parameters.AddWithValue("@TwoFAHash", twoFAHash);
		cmd.Parameters.AddWithValue("@TwoFADateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

		conn.Open();
		cmd.ExecuteNonQuery();
		conn.Close();
	}

	/// <summary>
	/// Sets email validated linked to the given username to true in the database 
	/// </summary>
	/// <param name="username"> The username to validate the email of </param>
	public static void ValidateEmail(string username)
	{
		string sql = @"UPDATE [Users] SET EmailConfirmed = 1 WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		cmd.ExecuteNonQuery();
		conn.Close();
	}

	/// <summary>
	/// Checks the username and password combination
	/// </summary>
	/// <param name="username"> The username </param>
	/// <param name="password"> The password </param>
	/// <returns> True if the pair exists, false otherwise </returns>
	public static bool CheckPassword(string username, string password)
	{
		// Hash and then salt the password
		byte[] salt = GetPasswordSalt(username);
		HasherSalter.HashSaltPassword(password, salt, out byte[] hash);

		byte[] saltedPasswordHash = GetSaltedPasswordHash(username);

		if (hash.Length != saltedPasswordHash.Length)
			return false;

		for (int i = 0; i < hash.Length; i++)
			if (hash[i] != saltedPasswordHash[i])
				return false;

		return true;
	}

	/// <summary>
	/// Increments how many times the user has won
	/// </summary>
	/// <param name="username"> The username </param>
	public static void IncrementWonTimes(string username)
	{
		GetStats(username, out int wonTimes, out _);

		string sql = @"UPDATE [Users] SET WonTimes = @WonTimes WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);
		cmd.Parameters.AddWithValue("@WonTimes", wonTimes + 1);

		conn.Open();
		cmd.ExecuteNonQuery();
		conn.Close();
	}

	/// <summary>
	/// Increments how many times the user has lost
	/// </summary>
	/// <param name="username"> The username </param>
	public static void IncrementLostTimes(string username)
	{
		GetStats(username, out _, out int lostTimes);

		string sql = @"UPDATE [Users] SET LostTimes = @LostTimes WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);
		cmd.Parameters.AddWithValue("@LostTimes", lostTimes + 1);

		conn.Open();
		cmd.ExecuteNonQuery();
		conn.Close();
	}

	/// <summary>
	/// Gets the stats linked to the username
	/// </summary>
	/// <param name="username"> Username to return the stats of </param>
	/// <param name="wonTimes"> An out parameter containing how many times the user has won </param>
	/// <param name="lostTimes"> An out parameter containing how many times the user has lost </param>
	public static void GetStats(string username, out int wonTimes, out int lostTimes)
	{
		string sql = @"SELECT WonTimes FROM [Users] WHERE Username = @Username";

		SqliteCommand cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		wonTimes = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
		conn.Close();

		sql = @"SELECT LostTimes FROM [Users] WHERE Username = @Username";

		cmd = conn.CreateCommand();
		cmd.CommandText = sql;

		cmd.Parameters.AddWithValue("@Username", username);

		conn.Open();
		lostTimes = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
		conn.Close();
	}
}