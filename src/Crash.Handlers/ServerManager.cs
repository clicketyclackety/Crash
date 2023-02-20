using Crash.Client;
using Crash.Common.Document;

namespace Crash.Utilities
{

	/// <summary>
	/// The server manager
	/// </summary>
	public static class ServerManager
	{
		// TODO : Move these consts
		public const string DefaultURL = "http://0.0.0.0";

		internal static int LastPort = 8080;

		/// <summary>
		/// Method to load the server
		/// </summary>
		/// <param name="url">the uri of the server</param>
		public static bool StartOrContinueLocalServer(CrashDoc crashDoc, string url)
		{
			CloseLocalServer(crashDoc);

			if (null == crashDoc) return false;
			crashDoc.LocalServer?.Stop();

			bool result = crashDoc.LocalServer.Start(url, out string resultMessage);

			Rhino.RhinoApp.WriteLine(resultMessage);

			return result;
		}

		public static void CloseLocalServer(CrashDoc crashDoc)
		{
			CrashServer? server = crashDoc?.LocalServer;
			if (null == server) return;

			server?.Stop();
			crashDoc.LocalServer.Dispose();
		}

		/// <summary>
		/// Checks for an active Server
		/// </summary>
		/// <returns>True if active, false otherwise</returns>
		public static bool CheckForActiveServer(CrashDoc crashDoc)
			=> crashDoc?.LocalServer is object;

	}

}
