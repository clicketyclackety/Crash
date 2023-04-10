using Crash.Handlers.Changes;

using Rhino.DocObjects;

namespace Crash.Utils
{

	/// <summary>Utilities for Change Objects.</summary>
	public static class ChangeUtils
	{
		private static string ChangeIdKey = "ChangeID";

		private static Dictionary<Guid, RhinoObject> RhinoChangeKeys;

		static ChangeUtils()
		{
			RhinoChangeKeys = new();
		}

		/// <summary>Acquires the ChangeId from the Rhino Object</summary>
		public static bool TryGetChangeId(this RhinoObject rObj, out Guid id)
		{
			id = Guid.Empty;
			if (rObj == null) return false;

			return rObj.UserDictionary.TryGetGuid(ChangeIdKey, out id);
		}

		/// <summary>Acquires the Rhino Object given the RhinoId from an IRhinoChange</summary>
		public static bool TryGetRhinoObject(this IChange change, out RhinoObject rhinoObject)
		{
			rhinoObject = default;
			if (change == null) return false;

			return RhinoChangeKeys.TryGetValue(change.Id, out rhinoObject);
		}

		/// <summary>Adds the ChangeId to the Rhino Object and vice Verse.</summary>
		public static void SyncHost(this RhinoObject rObj, IRhinoChange Change)
		{
			if (null == Change || rObj == null) return;

			if (rObj.UserDictionary.TryGetGuid(ChangeIdKey, out Guid changeId))
			{
				rObj.UserDictionary.Remove(ChangeIdKey);
				RhinoChangeKeys.Remove(changeId);
			}

			rObj.UserDictionary.Set(ChangeIdKey, (Change as IChange).Id);
			RhinoChangeKeys.Add(Change.Id, rObj);

			Change.RhinoId = rObj.Id;
		}

	}

}
