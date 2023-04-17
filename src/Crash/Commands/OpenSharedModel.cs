using Crash.Client;
using Crash.Common.Document;
using Crash.Communications;
using Crash.Handlers;

using Rhino.Commands;

namespace Crash.Commands
{

	/// <summary>Command to Open a Shared Model</summary>
	[CommandStyle(Style.ScriptRunner)]
	public sealed class OpenSharedModel : Command
	{

		private RhinoDoc rhinoDoc;
		private CrashDoc? crashDoc;

		private string LastURL = $"{CrashClient.DefaultURL}:{CrashServer.DefaultPort}";


		/// <summary>Default Constructor</summary>
		public OpenSharedModel()
		{
			Instance = this;
		}

		/// <inheritdoc />
		public static OpenSharedModel Instance { get; private set; }

		/// <inheritdoc />
		public override string EnglishName => "OpenSharedModel";

		/// <inheritdoc />
		protected override Result RunCommand(RhinoDoc doc, RunMode mode)
		{
			rhinoDoc = doc;
			crashDoc = CrashDocRegistry.GetRelatedDocument(doc);

			CommandUtils.CheckAlreadyConnected(crashDoc);

			if (!CommandUtils.GetUserName(out string name))
			{
				return Result.Cancel;
			}

			if (!_GetServerURL(ref LastURL))
			{
				RhinoApp.WriteLine("Invalid URL Input");
				return Result.Nothing;
			}

			crashDoc = CrashDocRegistry.CreateAndRegisterDocument(doc);
			_CreateCurrentUser(crashDoc, name);

			CommandUtils.StartLocalClient(crashDoc, LastURL);

			InteractivePipe.Active.Enabled = true;
			UsersForm.ShowForm();

			return Result.Success;
		}


		private bool _GetServerURL(ref string url)
			=> SelectionUtils.GetValidString("Server URL", ref url);

		private void _CreateCurrentUser(CrashDoc crashDoc, string name)
		{
			User user = new User(name);
			crashDoc.Users.CurrentUser = user;
		}

	}

}
