using Rhino.Commands;


namespace Crash.Commands
{

	/// <summary>Toggles the Crash Users UI</summary>
	public sealed class ToggleUsersUI : Command
	{

		/// <inheritdoc />
		public override string EnglishName => "ToggleCrashUI";

		/// <inheritdoc />
		protected override Result RunCommand(RhinoDoc doc, RunMode mode)
		{
			UsersForm.ToggleFormVisibility();
			return Result.Success;
		}

	}

}
