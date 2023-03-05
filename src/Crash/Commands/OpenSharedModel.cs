﻿using Crash.Client;
using Crash.Common.Document;
using Crash.Communications;
using Crash.Handlers;

using Rhino.Commands;

namespace Crash.Commands
{

	/// <summary>
	/// Command to Open a Shared Model
	/// </summary>
	public sealed class OpenSharedModel : Command
	{

		private RhinoDoc rhinoDoc;
		private CrashDoc? crashDoc;

		private string LastURL = $"{CrashClient.DefaultURL}:{CrashServer.DefaultPort}";


		/// <summary>
		/// Default Constructor
		/// </summary>
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

			// Check Crash Doc
			if (crashDoc?.LocalClient?.IsConnected == true)
			{
				RhinoApp.WriteLine("You are currently part of a Shared Model Session.");

				if (_NewModelOrExit(false) != true)
					return Result.Cancel;

				if (RhinoApp.RunScript(CloseSharedModel.Instance.EnglishName, true))
					RhinoApp.RunScript(OpenSharedModel.Instance.EnglishName, true);

				return Result.Success;
			}

			string name = Environment.UserName;
			if (!_GetUsersName(ref name))
			{
				RhinoApp.WriteLine("Invalid Name Input");
				return Result.Nothing;
			}

			if (!_GetServerURL(ref LastURL))
			{
				RhinoApp.WriteLine("Invalid URL Input");
				return Result.Nothing;
			}

			crashDoc = CrashDocRegistry.CreateAndRegisterDocument(doc);
			crashDoc.Queue.OnCompletedQueue += Queue_OnCompletedQueue;
			_CreateCurrentUser(crashDoc, name);

			// TODO : Ensure Requested Server is available, and notify if not
			ClientState clientState = new ClientState(crashDoc);
			string userName = crashDoc.Users.CurrentUser.Name;
			crashDoc.LocalClient = new CrashClient(crashDoc, userName, new Uri($"{LastURL}/Crash"));

			crashDoc.LocalClient.StartLocalClient(LocalClient_OnInitialize);

			InteractivePipe.Active.Enabled = true;

			return Result.Success;
		}

		private void LocalClient_OnInitialize(IEnumerable<Change> changes)
		{
			crashDoc.LocalClient.OnInitialize -= LocalClient_OnInitialize;

			Rhino.RhinoApp.WriteLine("Loading Changes ...");

			crashDoc.CacheTable.IsInit = true;

			try
			{
				ClientState state = new ClientState(crashDoc);
				state._HandleChangesAsync(changes);
			}
			catch
			{

			}
			finally
			{
				crashDoc.CacheTable.IsInit = false;
			}
		}

		private void Queue_OnCompletedQueue(object sender, EventArgs e)
		{
			crashDoc.Queue.OnCompletedQueue -= Queue_OnCompletedQueue;

			UsersForm.ToggleFormVisibility();
		}

		private bool _GetUsersName(ref string name)
			=> CommandUtils.GetValidString("Your Name", ref name);

		private bool? _NewModelOrExit(bool defaultValue)
			=> CommandUtils.GetBoolean(ref defaultValue,
				"Would you like to close this model?",
				"ExitCommand",
				"CloseModel");

		private bool _GetServerURL(ref string url)
			=> CommandUtils.GetValidString("Server URL", ref url);

		private void _CreateCurrentUser(CrashDoc crashDoc, string name)
		{
			User user = new User(name);
			crashDoc.Users.CurrentUser = user;
		}

	}

}
