﻿using Rhino.Commands;
using Rhino;
using Crash.Document;

namespace Crash.Commands
{

    /// <summary>
    /// Command to Open a Shared Model
    /// </summary>
    [CommandStyle(Style.DoNotRepeat | Style.NotUndoable | Style.ScriptRunner)]
    public sealed class OpenSharedModel : Command
    {

        private RhinoDoc _rDoc;

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
            _rDoc = doc;

            // Check Crash Doc
            if (ClientManager.CheckForActiveClient())
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

            string url = ClientManager.UrlAndPort;
            if (_GetServerURL(ref url))
            {
                _SetLastPortFromUrl(url);
                _SetLastUrlFromUrl(url);
            }
            else
            {
                RhinoApp.WriteLine("Invalid URL Input");
                return Result.Nothing;
            }

            new CrashDoc(doc);
            _CreateCurrentUser(name);

            // TODO : Ensure Requested Server is available, and notify if not
            Task.Run( () => ClientManager.StartOrContinueLocalClient(ClientManager.ClientUri));

            UsersForm.ToggleFormVisibility();

            return Result.Success;
        }

        private void _SetLastPortFromUrl(string url)
        {
            string[] ports = url.Replace("/","").Split(':');
            foreach (string port in ports)
            {
                if (!int.TryParse(port, out int givenPort)) continue;

                ClientManager.LastPort = givenPort;
                return;
            }

            ClientManager.LastPort = int.MinValue;
        }

        private void _SetLastUrlFromUrl(string urlWithPort)
        {
            string[] urlParts = urlWithPort.Split(':');
            string[] nonPortParts = urlParts.Where(v => !int.TryParse(v, out _)).ToArray();
            string url = string.Join(":", nonPortParts);

            if (!string.IsNullOrEmpty(url))
            {
                ClientManager.LastUrl = url;
            }
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

        private void _CreateCurrentUser(string name)
        {
            User user = new User(name);
            CrashDoc.ActiveDoc.Users.CurrentUser = user;
        }

    }

}
