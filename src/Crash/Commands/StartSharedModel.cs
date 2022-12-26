using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using Crash.Utilities;
using System.Drawing;
using System.Security.Policy;
using System.Threading;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO;
using Crash.UI;

namespace Crash.Commands
{

    /// <summary>
    /// Command to start the shared model
    /// </summary>
    [CommandStyle(Style.DoNotRepeat | Style.NotUndoable)]
    public sealed class StartSharedModel : Command
    {
        private static int lastPort = 5000;
        private const string defaultURL = "http://0.0.0.0:";

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
            if (RequestManager.LocalClient is object)
            {
                string closeCommand = CloseSharedModel.Instance.EnglishName;
                RhinoApp.WriteLine("You are currently part of a Shared Model Session. " +
                    $"Please restart Rhino to create or join a new session using the {closeCommand}.");

                return Result.Success;
            }

            string name = Environment.UserName;

            if (!_GetUsersName(ref name))
                return Result.Nothing;

            _CreateCurrentUser(name);

            if (!_GetPortFromUser(ref lastPort))
                return Result.Nothing;

            // TODO : Add Port Validation
            // TODO : Add Port Suggestions to docs

            // Start Server Host
            CrashServer.OnConnected += Server_OnConnected;
            CrashServer.OnFailure += Server_OnFailure;

            while(!ServerManager.StartOrContinueLocalServer($"{defaultURL}:{lastPort}"))
            {
                bool? close = _GetForceCloseOptions();
                if (close == null)
                    return Result.Cancel;

                else if (close.Value)
                    CrashServer.ForceCloselocalServers();
            }

            InteractivePipe.Instance.Enabled = true;

            return Result.Success;
        }

        private void Server_OnFailure(object sender, EventArgs e)
        {
            CrashServer.OnFailure -= Server_OnFailure;
            if (e is ErrorEventArgs errArgs)
            {
                RhinoApp.WriteLine(errArgs.GetException().Message);
            }
            else
            {
                RhinoApp.WriteLine("An Unknown Error occured");
            }
        }

        private void Server_OnConnected(object sender, EventArgs e)
        {
            CrashServer.OnConnected -= Server_OnConnected;
            try
            {
                // TODO : Create these urls/ports as constants somewhere relevent
                RequestManager.StartOrContinueLocalClient(new Uri($"http://127.0.0.1:{lastPort}/Crash"));
            }
            catch (UriFormatException)
            {
                RhinoApp.Write("Please enter a valid host! The Port is likely bad.");
            }
            catch(Exception ex)
            {
                RhinoApp.WriteLine(ex.Message);
            }
        }

        internal static bool? _GetForceCloseOptions()
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
        internal static bool _GetUsersName(ref string name)
            => CommandUtils.GetValidString("Your Name", ref name);

        internal static bool _GetPortFromUser(ref int port)
            => CommandUtils.GetInteger("Server port", ref port);

        internal static void _CreateCurrentUser(string name)
        {
            User user = new User(name);
            User.CurrentUser = user;
        }

    }

}
