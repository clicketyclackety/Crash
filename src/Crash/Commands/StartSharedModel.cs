using Crash.Client;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Communications;
using Crash.Handlers;
using Crash.Handlers.InternalEvents;
using Crash.Handlers.Plugins;

using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Input.Custom;

namespace Crash.Commands
{

	/// <summary>
	/// Command to start the shared model
	/// </summary>
	public sealed class StartSharedModel : Command
	{
		private RhinoDoc rhinoDoc;
		private CrashDoc crashDoc;

		private int LastPort = CrashServer.DefaultPort;
		private string LastServerURL = CrashServer.DefaultURL;
		private string LastClientURL = CrashClient.DefaultURL;
		private string LastServerURLAndPort => $"{LastServerURL}:{LastPort}";
		private string LastClientURLAndPort => $"{LastClientURL}:{LastPort}/Crash";

		private bool includePreExistingGeometry = false;

		/// <summary>
		/// Empty constructor
		/// </summary>
		public StartSharedModel()
		{
			Instance = this;
		}

		/// <summary>
		/// Command Instance
		/// </summary>
		public static StartSharedModel Instance { get; private set; }

		/// <inheritdoc />
		public override string EnglishName => "StartSharedModel";

		/// <inheritdoc />
		protected override Result RunCommand(RhinoDoc doc, RunMode mode)
		{
			rhinoDoc = doc;
			crashDoc = CrashDocRegistry.GetRelatedDocument(doc);

			if (!CommandUtils.CheckForRunningServer(crashDoc))
				return Result.Cancel;

			if (!CommandUtils.GetUserName(out string name))
			{
				return Result.Cancel;
			}

			// TODO : Add Port Validation
			// TODO : Add Port Suggestions to docs
			if (!_GetPortFromUser(ref LastPort))
			{
				RhinoApp.WriteLine("Invalid Port!");
				return Result.Nothing;
			}

			crashDoc = CrashDocRegistry.CreateAndRegisterDocument(doc);

			_CreateCurrentUser(name);

#if DEBUG
			if (_PreExistingGeometryCheck(doc))
			{
				includePreExistingGeometry = _ContinueOrQuit() == true;
			}
#endif

			try
			{
				crashDoc.LocalServer = new CrashServer(crashDoc);

				crashDoc.LocalServer.OnConnected += Server_OnConnected;
				crashDoc.LocalServer.OnFailure += Server_OnFailure;

				crashDoc.LocalServer.Start(LastServerURLAndPort);

				InteractivePipe.Active.Enabled = true;
				UsersForm.ShowForm();
			}
			catch (Exception ex)
			{
				RhinoApp.WriteLine("The server ran into difficulties starting.");
				RhinoApp.WriteLine($"More specifically ; {ex.Message}.");

				if (_GetForceCloseOptions() != true) return Result.Cancel;
				if (!CrashServer.ForceCloselocalServers(1000)) return Result.Cancel;
				RhinoApp.RunScript(EnglishName, true);
			}

			return Result.Success;
		}

		private void AddPreExistingGeometry(CrashDoc crashDoc)
		{
			string? user = crashDoc.Users?.CurrentUser.Name;
			if (string.IsNullOrEmpty(user))
			{
				RhinoApp.WriteLine("User is invalid!");
				return;
			}

			RhinoDoc rhinoDoc = CrashDocRegistry.GetRelatedDocument(crashDoc);

			var enumer = GetObjects(rhinoDoc).GetEnumerator();
			while (enumer.MoveNext())
			{
				var args = new CrashObjectEventArgs(enumer.Current);
				EventDispatcher.Instance.NotifyDispatcher(ChangeAction.Add, this, args, rhinoDoc);
			}
		}

		private bool? _ContinueOrQuit(bool defaultValue = false)
			=> SelectionUtils.GetBoolean(ref defaultValue,
				"Would you like to include preExisting Geometry?",
				"dontInclude",
				"include");

		private IEnumerable<RhinoObject> GetObjects(RhinoDoc doc)
		{
			ObjectEnumeratorSettings settings = new ObjectEnumeratorSettings()
			{
				ActiveObjects = true,
				DeletedObjects = true,
				HiddenObjects = true,
				IncludeGrips = false,
				IncludeLights = false,
				LockedObjects = true,
				NormalObjects = true,
			};
			return doc.Objects.GetObjectList(settings);
		}

		private bool _PreExistingGeometryCheck(RhinoDoc doc)
			=> GetObjects(doc).Count() > 0;

		private void Server_OnFailure(object sender, CrashEventArgs e)
		{
			if (e.CrashDoc.LocalServer is object)
			{
				e.CrashDoc.LocalServer.OnFailure -= Server_OnFailure;
			}

			string message = "An Unknown Error occured";

			RhinoApp.WriteLine(message);
		}

		private void Server_OnConnected(object sender, CrashEventArgs e)
		{
			if (null == e) return;

			e.CrashDoc.LocalServer.OnConnected -= Server_OnConnected;

			try
			{
				string userName = e.CrashDoc.Users.CurrentUser.Name;
				var crashClient = new CrashClient(e.CrashDoc, userName, new Uri(LastClientURLAndPort));
				e.CrashDoc.LocalClient = crashClient;

				crashClient.StartLocalClientAsync();

				if (includePreExistingGeometry)
					AddPreExistingGeometry(e.CrashDoc);
			}
			catch (UriFormatException)
			{
				RhinoApp.Write("Please enter a valid host! The Port is likely bad.");
			}
			catch (Exception ex)
			{
				RhinoApp.WriteLine(ex.Message);
			}
		}

		private bool? _GetForceCloseOptions()
		{
			bool defaultValue = false;

			GetOption go = new GetOption();
			go.AcceptEnterWhenDone(true);
			go.AcceptNothing(true);
			go.SetCommandPrompt("Would you like to Force Close any other servers?");
			OptionToggle releaseValue = new OptionToggle(defaultValue, "NoThanks", "CloseAll");
			int releaseIndex = go.AddOptionToggle("Close", ref releaseValue);

			while (true)
			{
				GetResult result = go.Get();
				if (result == GetResult.Option)
				{
					int index = go.OptionIndex();
					if (index == releaseIndex)
					{
						defaultValue = !defaultValue;
					}
				}
				else if (result == GetResult.Cancel)
				{
					return null;
				}
				else if (result == GetResult.Nothing)
				{
					return defaultValue;
				}
			}


		}

		// TODO : Ensure name is not already taken!
		private bool _GetUsersName(ref string name)
			=> SelectionUtils.GetValidString("Your Name", ref name);

		private bool _GetPortFromUser(ref int port)
			=> SelectionUtils.GetInteger("Server port", ref port);

		private void _CreateCurrentUser(string name)
		{
			User user = new User(name);
			crashDoc.Users.CurrentUser = user;
		}

	}

}
