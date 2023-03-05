using Crash.Handlers.Changes;

namespace Crash.Utils
{

	/// <summary>Utilities for Change Objects.</summary>
	public static class ChangeUtils
	{
		private static string ChangeIdKey = "ChangeID";

		/// <summary>Acquires the ChangeId from the Rhino Object</summary>
		public static bool TryGetChangeId(Rhino.DocObjects.RhinoObject rObj, out Guid id)
		{
			id = Guid.Empty;
			if (rObj == null) return false;

			return rObj.UserDictionary.TryGetGuid(ChangeIdKey, out id);
		}

		/// <summary>Adds the ChangeId to the Rhino Object and vice Verse.</summary>
		public static void SyncHost(Rhino.DocObjects.RhinoObject rObj, IRhinoChange Change)
		{
			if (null == Change || rObj == null) return;

			// Data
			if (rObj.UserDictionary.TryGetGuid(ChangeIdKey, out _))
			{
				rObj.UserDictionary.Remove(ChangeIdKey);
			}

			rObj.UserDictionary.Set(ChangeIdKey, Change.Id);

			Change.RhinoId = rObj.Id;
		}

	}

}
