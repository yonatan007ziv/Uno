using GameEngine;
using GameEngine.Services.Interfaces;
using Uno.Client.Scenes;

namespace Uno.Client;

internal class Program
{
	/// <summary>
	/// Entry point: Fires up the game engine and loads up the LoginRegister scene
	/// </summary>
	public static void Main()
	{
		IGameEngine GameEngine = new GameEngineProvider().UseSilkOpenGL().BuildEngine();

		GameEngine.Title = "Uno";
		GameEngine.SetResourceFolder(@$"{Directory.GetCurrentDirectory()}\GameResources");
		GameEngine.SetWindowBackgroundColor(System.Drawing.Color.CadetBlue);

		GameEngine.LogRenderingLogs = false;

		new LoginRegisterScene().LoadScene();

		GameEngine.Run();
	}
}