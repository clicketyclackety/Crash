using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crash.Common.Serialization
{

	/// <summary>Default Internal Json Serialization.
	/// This has nothing to do with the SignalR Hub</summary>
	internal static class Options
	{

		static Options()
		{
			Default = new JsonSerializerOptions()
			{
				IgnoreReadOnlyFields = true,
				IgnoreReadOnlyProperties = true,
				IncludeFields = true,
				NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
				ReadCommentHandling = JsonCommentHandling.Skip,
				WriteIndented = true,
			};
		}

		internal readonly static JsonSerializerOptions Default;

	}

}
