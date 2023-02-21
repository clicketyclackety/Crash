﻿using Crash.Client;
using Crash.Common.Changes;
using Crash.Common.Document;
using Crash.Common.Events;
using Crash.Communications;
using Crash.Handlers;

using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace Crash.Commands
{

	/// <summary>
	/// Command to start the shared model
	/// </summary>
	[CommandStyle(Style.DoNotRepeat | Style.NotUndoable)]
	public sealed class StartSharedModel : Command
	{
		private RhinoDoc rhinoDoc;
		private CrashDoc crashDoc;

		private int LastPort = CrashServer.DefaultPort;
		private string LastURL = CrashServer.DefaultURL;
		private string LastURLAndPort => $"{LastURL}:{LastPort}";

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
			if (crashDoc?.LocalClient is object || crashDoc.LocalServer.IsRunning)
			{
				string closeCommand = CloseSharedModel.Instance.EnglishName;
				RhinoApp.WriteLine("You are currently part of a Shared Model Session. " +
					$"Please restart Rhino to create or join a new session using the {closeCommand}.");

				return Result.Success;
			}

			string name = Environment.UserName;

			if (!_GetUsersName(ref name))
			{
				RhinoApp.WriteLine("Invalid name!");
				return Result.Nothing;
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

			if (_PreExistingGeometryCheck(doc))
			{
				includePreExistingGeometry = _ContinueOrQuit() == true;
			}

			// Start Server Host
			crashDoc.LocalServer.OnConnected += Server_OnConnected;
			crashDoc.LocalServer.OnFailure += Server_OnFailure;

			while (crashDoc.LocalServer.IsRunning)
			{
				bool? close = _GetForceCloseOptions();
				if (close == null)
					return Result.Cancel;

				else if (close == true)
				{
					CrashServer.ForceCloselocalServers();
					break;
				}
			}

			try
			{
				crashDoc.LocalServer.Start(LastURLAndPort);
				crashDoc.Queue.OnCompletedQueue += Queue_OnCompletedQueue;
			}
			catch
			{
				RhinoApp.WriteLine("The server ran into difficulties starting");
			}

			return Result.Success;
		}

		private void Queue_OnCompletedQueue(object sender, EventArgs e)
		{
			crashDoc.Queue.OnCompletedQueue -= Queue_OnCompletedQueue;

			UsersForm.ToggleFormVisibility();
		}

		private void AddPreExistingGeometry(CrashDoc crashDoc)
		{
			string? user = crashDoc.Users?.CurrentUser.Name;
			if (string.IsNullOrEmpty(user))
			{
				RhinoApp.WriteLine("User is invalid!");
				return;
			}

			var enumer = GetObjects(RhinoDoc.ActiveDoc).GetEnumerator();
			while (enumer.MoveNext())
			{
				GeometryBase geom = enumer.Current.Geometry;
				GeometryChange Change = GeometryChange.CreateNew(user, geom);
				crashDoc.CacheTable?.UpdateChangeAsync(Change);
			}
		}

		private bool? _ContinueOrQuit(bool defaultValue = false)
			=> CommandUtils.GetBoolean(ref defaultValue,
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
			e.CrashDoc.LocalServer.OnConnected -= Server_OnConnected;

			try
			{
				ClientState clientState = new ClientState(crashDoc);
				CrashClient.StartOrContinueLocalClient(crashDoc, new Uri(LastURLAndPort), clientState.Init);

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
			=> CommandUtils.GetValidString("Your Name", ref name);

		private bool _GetPortFromUser(ref int port)
			=> CommandUtils.GetInteger("Server port", ref port);

		private void _CreateCurrentUser(string name)
		{
			User user = new User(name);
			crashDoc.Users.CurrentUser = user;
		}

	}

}
