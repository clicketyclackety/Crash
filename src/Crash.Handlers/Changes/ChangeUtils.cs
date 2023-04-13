using Rhino;
using Rhino.DocObjects;

namespace Crash.Utils
{

	/// <summary>Utilities for Change Objects.</summary>
	public static class ChangeUtils
	{
		private static string ChangeIdKey = "ChangeID";

		// TODO : Not multi-doc compatible!!!
		private static Dictionary<Guid, RhinoObject> RhinoChangeKeys;
		private static HashSet<Guid> SelectedObjects;
		internal static void ClearSelected() => SelectedObjects.Clear();
		internal static HashSet<Guid> GetSelected() => SelectedObjects;

		static ChangeUtils()
		{
			RhinoChangeKeys = new();
			SelectedObjects = new();
			RhinoDoc.SelectObjects += (sender, args) =>
			{
				foreach (var obj in args.RhinoObjects)
				{
					if (!TryGetChangeId(obj, out Guid ChangeId)) continue;
					SelectedObjects.Add(ChangeId);
				}
			};
			RhinoDoc.DeselectObjects += (sender, args) =>
			{
				foreach (var obj in args.RhinoObjects)
				{
					if (!TryGetChangeId(obj, out Guid ChangeId)) continue;
					SelectedObjects.Remove(ChangeId);
				}
			};
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
		public static void SyncHost(this RhinoObject rObj, IChange Change)
		{
			if (null == Change || rObj == null) return;

			if (rObj.UserDictionary.TryGetGuid(ChangeIdKey, out Guid changeId))
			{
				rObj.UserDictionary.Remove(ChangeIdKey);
				RhinoChangeKeys.Remove(changeId);
			}

			rObj.UserDictionary.Set(ChangeIdKey, Change.Id);
			RhinoChangeKeys.Add(Change.Id, rObj);
		}

		/// <summary>Check for Oversied Payload</summary>
		public static bool IsOversized(this IChange change)
			=> change.Payload?.Length > ushort.MaxValue;

	}

}
