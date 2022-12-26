using Rhino.Input.Custom;
using Rhino.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Render.Fields;
using Rhino.Commands;

namespace Crash.Commands
{

    /// <summary>
    /// Reusable Utilities for commands
    /// </summary>
    internal static class CommandUtils
    {

        internal static bool? GetBoolean(ref bool defaultValue, string prompt, string offValue, string onValue)
        {
            GetOption go = new GetOption();
            go.AcceptEnterWhenDone(true);
            go.AcceptNothing(true);
            go.SetCommandPrompt(prompt);
            OptionToggle releaseValue = new OptionToggle(defaultValue, offValue, onValue);
            int releaseIndex = go.AddOptionToggle("Release", ref releaseValue);

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

        internal static bool GetValidString(string prompt, ref string value)
        {
            Result getUrl = RhinoGet.GetString(prompt, true, ref value);

            if (string.IsNullOrEmpty(value)) return false;

            return getUrl == Result.Success;
        }

        internal static bool GetInteger(string prompt, ref int value)
        {
            Result getPort = RhinoGet.GetInteger(prompt, false, ref value);
            if (value <= 0) return false;

            return getPort == Result.Success;
        }

    }

}
