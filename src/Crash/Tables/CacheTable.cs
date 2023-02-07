using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino;
using Crash.Document;
using System.Collections;
using SpeckLib;
using System.Collections.Generic;
using Crash.Events;
using Crash.Events.Args;

namespace Crash.Tables
{
    /// <summary>
    /// Local cache of the collaboration database
    /// </summary>
    public sealed class CacheTable : IEnumerable<SpeckInstance>
    {
        public bool SomeoneIsDone { get; set; }
        public bool IsInit { get; set; }
        private static string SpeckIdKey = "SPECKID";

        private ConcurrentDictionary<Guid, SpeckInstance> _cache { get; set; }

        //                        <SpeckId, RhinoId>
        private ConcurrentDictionary<Guid, Guid> _SpeckToRhino { get; set; }

        /// <summary>
        /// Local cache constructor subscribing to RhinoApp_Idle
        /// </summary>
        public CacheTable()
        {
            _cache = new ConcurrentDictionary<Guid, SpeckInstance>();
            _SpeckToRhino = new ConcurrentDictionary<Guid, Guid>();
        }
            
        internal void Clear()
        {
            _SpeckToRhino?.Clear();
            _cache?.Clear();
        }

        #region ConcurrentDictionary Methods

        /// <summary>
        /// Method to update a speck
        /// </summary>
        /// <param name="speck">the specks</param>
        /// <returns>returns the update task</returns>
        public async Task UpdateSpeck(SpeckInstance speck)
        {
            if (speck == null) return;

            if (_cache.ContainsKey(speck.Id))
            {
                _cache.TryRemove(speck.Id, out _);
            }

            _cache.TryAdd(speck.Id, speck);
            if (string.IsNullOrEmpty(speck.Owner))
                return;
        }

        /// <summary>
        /// Method to get specks
        /// </summary>
        /// <returns>returns a list of the specks</returns>
        public IEnumerable<SpeckInstance> GetSpecks()
        {
            return _cache.Values;
        }

        public RhinoObject GetHost(SpeckInstance speck)
        {
            if (!_SpeckToRhino.TryGetValue(speck.Id, out Guid hostId)) return null;

            // FIXME : Not a good idea for Mac
            return RhinoDoc.ActiveDoc.Objects.Find(hostId);
        }

        public Guid GetHost(Guid speckId)
        {
            _SpeckToRhino.TryGetValue(speckId, out Guid hostId);
            return hostId;
        }

#endregion

        private void BakeSpeck(EventArgs args)
        {
            if (args is not BakeArgs bakeArgs) return;

            GeometryBase? geom = bakeArgs.Geometry;
            if (null == geom) return;

            Guid id = bakeArgs.Doc.Objects.Add(geom);
            if (Guid.Empty == id) return;

            RhinoObject rObj = bakeArgs.Doc.Objects.Find(id);

            SyncHost(rObj, bakeArgs.Speck);
        }

        /// <summary>
        /// Bake the specks to rhino
        /// </summary>
        /// <param name="speck">the speck to bake</param>
        internal void QueueSpeckBake(SpeckInstance speck)
        {
            if (speck == null) return;

            var args = new BakeArgs(RhinoDoc.ActiveDoc, speck);
            CrashDoc.ActiveDoc.Queue.AddAction(new Events.IdleAction(BakeSpeck, args));
        }

        public static Guid? GetSpeckId(RhinoObject rObj)
        {
            if (rObj == null) return null;

            if (rObj.UserDictionary.TryGetGuid(SpeckIdKey, out var key))
                return key;

            return null;
        }

        public void SyncHost(RhinoObject rObj, ISpeck speck)
        {
            if (null == speck || rObj == null) return;

            // Data
            if (rObj.UserDictionary.TryGetGuid(SpeckIdKey, out _))
            {
                rObj.UserDictionary.Remove(SpeckIdKey);
            }

            rObj.UserDictionary.Set(SpeckIdKey, speck.Id);

            // Key/Key
            if (_SpeckToRhino.ContainsKey(speck.Id))
            {
                _SpeckToRhino.TryRemove(speck.Id, out _);
            }
            _SpeckToRhino.TryAdd(speck.Id, rObj.Id);
        }

        /// <summary>
        /// Bake multiple specks to rhino
        /// </summary>
        /// <param name="specks">the enumerable specks to bake</param>
        internal void QueueBakeSpecks(IEnumerable<SpeckInstance> specks)
        {
            var enumer = specks.GetEnumerator();
            while (enumer.MoveNext())
            {
                QueueSpeckBake(enumer.Current);
            }
            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        #region delete specks

        /// <summary>
        /// Delete a speck from rhino
        /// </summary>
        /// <param name="speck">the speck to delete</param>
        /// This needs to happen in Idle/
        void QueueDeleteSpeck(Guid speckId)
        {
            DeCacheSpeck(speckId);

            // TODO : Don't use ActiveDoc
            DeleteArgs deleteArgs = new DeleteArgs(RhinoDoc.ActiveDoc, speckId);
            IdleAction idleAction = new IdleAction(DeleteSpeck, deleteArgs);
            CrashDoc.ActiveDoc.Queue.AddAction(idleAction);
        }

        private void DeleteSpeck(EventArgs args)
        {
            if (args is not DeleteArgs delArgs) return;

            Guid hostId = GetHost(delArgs.SpeckId);
            delArgs.Doc.Objects.Delete(hostId, true);
        }

        /// <summary>
        /// Delete multiple specks from rhino
        /// </summary>
        /// <param name="specks">the specks to delete</param>
        void QueueDeleteSpecks(IEnumerable<SpeckInstance> specks)
        {
            if (null == specks) return;

            var enumer = specks.GetEnumerator();
            while (enumer.MoveNext())
            {
                QueueDeleteSpeck(enumer.Current.Id);
            }
        }

        #endregion

        #region Remove Specks

        /// <summary>
        /// Remove a speck for the cache
        /// </summary>
        /// <param name="speck">the speck to remove</param>
        internal void DeCacheSpeck(Guid speckId)
        {
            _cache.TryRemove(speckId, out _);
        }

        /// <summary>
        /// Remove multiple specks from the cache
        /// </summary>
        /// <param name="specks">the specks to remove</param>
        internal void DeCacheSpecks(IEnumerable<ISpeck> specks)
        {
            if (null == specks) return;

            var enumer = specks.GetEnumerator();
            while (enumer.MoveNext())
            {
                DeCacheSpeck(enumer.Current.Id);
            }
        }

        #endregion

        #region Event Listeners
        /// <summary>
        /// On add event. A user has added an element elsewhere.
        /// </summary>
        /// <param name="name">the name </param>
        /// <param name="speck">the speck</param>
        internal void OnAdd(string name, ISpeck speck)
        {
            if (null == speck) return;

            CrashDoc.ActiveDoc?.Users.Add(speck.Owner);

            SpeckInstance lSpeck = new SpeckInstance(speck);
            UpdateSpeck(lSpeck);
        }

        /// <summary>
        /// On delete event. A user has deleted an element elsewhere.
        /// </summary>
        /// <param name="owner">the name</param>
        /// <param name="speckId">speck id</param>
        internal void OnDelete(string owner, Guid speckId)
        {
            if (Guid.Empty == speckId || string.IsNullOrEmpty(owner)) return;

            // SpeckInstance speck = new SpeckInstance(new Speck(speckId, owner, null));
            QueueDeleteSpeck(speckId);
            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        /// <summary>
        /// On update event
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="speckID">the speck id</param>
        /// <param name="speck">the speck</param>
        internal void OnUpdate(string name, Guid speckId, SpeckInstance speck)
        {
            if (null == speck ||
                Guid.Empty == speckId ||
                string.IsNullOrEmpty(name)) return;

            // TODO : ...
        }

        /// <summary>
        /// Collaboration is done event. A user has called the Release command.
        /// </summary>
        /// <param name="name">The name of the collaboration</param>
        public void CollaboratorIsDone(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            var cacheTable = CrashDoc.ActiveDoc?.CacheTable;
            if (null == cacheTable) return;

            SomeoneIsDone = true;
            string? sanitisedName = name?.ToLower();
            IEnumerable<SpeckInstance> ToBake = cacheTable.GetSpecks()
                                        .Where(s => s.Owner?.ToLower() == sanitisedName)
                                        .Where(s => s is object);

            cacheTable.QueueBakeSpecks(ToBake);
            cacheTable.DeCacheSpecks(ToBake);
            SomeoneIsDone = false;

            CrashDoc.ActiveDoc?.Users.Remove(name);
            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        public IEnumerator<SpeckInstance> GetEnumerator() => _cache.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }
}
