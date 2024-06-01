using Uno.Core.Utilities.Models;

namespace Uno.Core.Utilities;

public class DevConstants
{
	// Email
	private const string emailHost = "";
	private const string emailPassword = "";
	public static EmailCredentials EmailCredentials { get; } = new EmailCredentials() { Host = emailHost, Password = emailPassword };

	// Database
	private static string sqliteDbConnString { get; } = ""; // @"Data Source=ABSOLUTE_PATH_TO_DB"
	public static DatabaseCredentials DatabaseCredentials { get; } = new DatabaseCredentials() { ConnectionString = sqliteDbConnString };

	// Server addresses
	public static string LoginRegisterServerAddress { get; } = "127.0.0.1";
	public static int LoginRegisterServerPort { get; } = 25000;
	public static string GameplayServerAddress { get; } = "127.0.0.1";
	public static int GameplayServerPort { get; } = 25001;
}