using System.IO;

using Rhino.Input.Custom;
using Rhino.DocObjects;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino;
using Crash.Tables;
using Crash.Document;

namespace Crash.Commands
{

    /// <summary>
    /// Command to start the shared model
    /// </summary>
    [CommandStyle(Style.DoNotRepeat | Style.NotUndoable)]
    public sealed class StartSharedModel : Command
    {
        private RhinoDoc _rDoc;

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
            _rDoc = doc;

            if (ClientManager.CheckForActiveClient() || ServerManager.CheckForActiveServer())
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
            _CreateCurrentUser(name);

            // TODO : Add Port Validation
            // TODO : Add Port Suggestions to docs
            if (!_GetPortFromUser(ref ServerManager.LastPort))
            {
                RhinoApp.WriteLine("Invalid Port!");
                return Result.Nothing;
            }

            if (_PreExistingGeometryCheck(doc))
            {
                includePreExistingGeometry = _ContinueOrQuit() == true;
            }

            // Start Server Host
            CrashServer.OnConnected += Server_OnConnected;
            CrashServer.OnFailure += Server_OnFailure;

            while(!ServerManager.StartOrContinueLocalServer($"{ServerManager.DefaultURL}:{ServerManager.LastPort}"))
            {
                bool? close = _GetForceCloseOptions();
                if (close == null)
                    return Result.Cancel;

                else if (close.Value)
                    CrashServer.ForceCloselocalServers();
            }

            InteractivePipe.Instance.Enabled = true;

            UsersForm.ToggleFormVisibility();

            return Result.Success;
        }

        private void AddPreExistingGeometry()
        {
            string? user = CrashDoc.ActiveDoc?.Users?.CurrentUser?.Name;
            if (string.IsNullOrEmpty(user))
            {
                RhinoApp.WriteLine("User is invalid!");
                return;
            }

            var enumer = GetObjects(RhinoDoc.ActiveDoc).GetEnumerator();
            while(enumer.MoveNext())
            {
                GeometryBase geom = enumer.Current.Geometry;
                SpeckInstance speck = SpeckInstance.CreateNew(user, geom);
                CacheTable.Instance.UpdateSpeck(speck);
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

        private void Server_OnFailure(object sender, EventArgs e)
        {
            CrashServer.OnFailure -= Server_OnFailure;

            string message = "An Unknown Error occured";

            if (e is ErrorEventArgs errArgs)
            {
                message = errArgs.GetException().Message;
            }

            RhinoApp.WriteLine(message);
        }

        private void Server_OnConnected(object sender, EventArgs e)
        {
            CrashServer.OnConnected -= Server_OnConnected;
            try
            {
                ClientManager.StartOrContinueLocalClient(ClientManager.ClientUri);

                if (includePreExistingGeometry)
                    AddPreExistingGeometry();
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
            CrashDoc.ActiveDoc.Users.CurrentUser = user;
        }

    }

}
