using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;

namespace Crash.Commands
{

	/// <summary>Reusable Utilities for commands</summary>
	internal static class SelectionUtils
	{

		/// <summary>
		/// Asks a user to toggle between two options
		/// </summary>
		/// <param name="defaultValue">The default value iput</param>
		/// <param name="prompt">The Prompt for the User</param>
		/// <param name="offValue">The message for false</param>
		/// <param name="onValue">The message for true</param>
		/// <returns>null on Cancel. True or False from the user choice</returns>
		internal static bool? GetBoolean(ref bool defaultValue, string prompt, string offValue, string onValue)
		{
			GetOption go = new GetOption();
			go.AcceptEnterWhenDone(true);
			go.AcceptNothing(true);
			go.SetCommandPrompt(prompt);
			OptionToggle releaseValue = new OptionToggle(defaultValue, offValue, onValue);
			int releaseIndex = go.AddOptionToggle("Choose", ref releaseValue);

			while (true)
			{
				GetResult result = go.Get();
				if (result == GetResult.Option && go.OptionIndex() == releaseIndex)
					defaultValue = !defaultValue;

				else if (result == GetResult.Cancel)
					return null;

				else if (result == GetResult.Nothing)
					return defaultValue;
			}
		}

		/// <summary>
		/// Asks the user to input a string
		/// </summary>
		/// <param name="prompt">The Prompt for the User</param>
		/// <param name="value">The chosen Value, can set a default</param>
		/// <returns>bool on success, false on cancel or empty value</returns>
		internal static bool GetValidString(string prompt, ref string value)
		{
			Result getUrl = RhinoGet.GetString(prompt, true, ref value);
			if (string.IsNullOrEmpty(value)) return false;

			return getUrl == Result.Success;
		}

		/// <summary>
		/// Asks the user to input an integer
		/// </summary>
		/// <param name="prompt">The Prompt for the User</param>
		/// <param name="value">The chosen Value, can set a default</param>
		/// <returns>bool on success, false on cancel or integer < 0</returns>
		internal static bool GetInteger(string prompt, ref int value)
		{
			Result getPort = RhinoGet.GetInteger(prompt, false, ref value);
			if (value <= 0) return false;

			return getPort == Result.Success;
		}

	}

}
