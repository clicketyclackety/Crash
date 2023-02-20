using System;


namespace Crash.Utils
{

    public static class ChangeUtils
    {
        private static string ChangeIdKey = "ChangeID";

        public static Guid? GetChangeId(Rhino.DocObjects.RhinoObject rObj)
        {
            if (rObj == null) return null;

            if (rObj.UserDictionary.TryGetGuid(ChangeIdKey, out var key))
                return key;

            return null;
        }

        public static void SyncHost(Rhino.DocObjects.RhinoObject rObj, IChange Change)
        {
            if (null == Change || rObj == null) return;

            // Data
            if (rObj.UserDictionary.TryGetGuid(ChangeIdKey, out _))
            {
                rObj.UserDictionary.Remove(ChangeIdKey);
            }

            rObj.UserDictionary.Set(ChangeIdKey, Change.Id);

            /* REPLACE IN CacheTable
            // Key/Key
            if (_ChangeToRhino.ContainsKey(Change.Id))
            {
                _ChangeToRhino.TryRemove(Change.Id, out _);
            }
            _ChangeToRhino.TryAdd(Change.Id, rObj.Id);
            */
        }
    }

}
