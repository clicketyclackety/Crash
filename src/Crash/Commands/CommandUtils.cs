using System.Threading.Tasks;

using Crash.Client;
using Crash.Common.Document;

using Rhino.Commands;

namespace Crash.Commands
{

	public static class CommandUtils
	{

		public static Result CheckAlreadyConnected(CrashDoc crashDoc)
		{
			if (crashDoc?.LocalClient?.IsConnected == true)
			{
				RhinoApp.WriteLine("You are currently part of a Shared Model Session.");

				if (_NewModelOrExit(false) != true)
					return Result.Cancel;

				if (RhinoApp.RunScript(CloseSharedModel.Instance.EnglishName, true))
					RhinoApp.RunScript(OpenSharedModel.Instance.EnglishName, true);
			}

			return Result.Success;
		}

		private static bool? _NewModelOrExit(bool defaultValue)
			=> SelectionUtils.GetBoolean(ref defaultValue,
				"Would you like to close this model?",
				"ExitCommand",
				"CloseModel");

		public static bool GetUserName(out string name)
		{
			name = Environment.UserName;
			if (!_GetUsersName(ref name))
			{
				RhinoApp.WriteLine("Invalid Name Input");
				return false;
			}

			return true;
		}

		private static bool _GetUsersName(ref string name)
			=> SelectionUtils.GetValidString("Your Name", ref name);

		public static async Task StartLocalClient(CrashDoc crashDoc, string url)
		{
			// TODO : Ensure Requested Server is available, and notify if not
			string userName = crashDoc.Users.CurrentUser.Name;
			var crashClient = new CrashClient(crashDoc, userName, new Uri($"{url}/Crash"));
			crashDoc.LocalClient = crashClient;

			await crashClient.StartLocalClientAsync();
		}

		public static bool CheckForRunningServer(CrashDoc crashDoc)
		{
			if (crashDoc?.LocalServer is object && crashDoc.LocalServer.IsRunning)
			{
				string closeCommand = CloseSharedModel.Instance.EnglishName;
				RhinoApp.WriteLine("You are currently part of a Shared Model Session. " +
					$"Please use the {closeCommand} command.");

				return false;
			}

			return true;
		}

	}

}
