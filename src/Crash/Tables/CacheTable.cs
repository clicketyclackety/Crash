using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino;
using Crash.Document;

namespace Crash.Tables
{
    /// <summary>
    /// Local cache of the collaboration database
    /// </summary>
    public sealed class CacheTable
    {
        public bool SomeoneIsDone { get; set; }
        public bool IsInit { get; set; }
        private static string SpeckIdKey = "SPECKID";

        private ConcurrentDictionary<Guid, SpeckInstance> _cache { get; set; }

        //                        <SpeckId, RhinoId>
        private ConcurrentDictionary<Guid, Guid> _SpeckToRhino { get; set; }

        private List<SpeckInstance> ToBake = new List<SpeckInstance>();
        private List<SpeckInstance> ToRemove = new List<SpeckInstance>();

        /// <summary>
        /// The instance of the local cache
        /// </summary>
        [Obsolete("Obsolete", true)]
        public static CacheTable Instance { get; set; }

        /// <summary>
        /// Local cache constructor subscribing to RhinoApp_Idle
        /// </summary>
        public CacheTable()
        {
            RhinoApp.Idle += RhinoApp_Idle;
            _cache = new ConcurrentDictionary<Guid, SpeckInstance>();
            _SpeckToRhino = new ConcurrentDictionary<Guid, Guid>();
        }

        internal void Clear()
        {
            ToBake?.Clear();
            ToRemove?.Clear();
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

            // TODO : View should be redrawn on Update,
            // but this would make things slow
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

        #region bake specks
        /// <summary>
        /// Bake the specks to rhino
        /// </summary>
        /// <param name="speck">the speck to bake</param>
        internal void BakeSpeck(SpeckInstance speck)
        {
            if (speck == null) return;

            var _doc = RhinoDoc.ActiveDoc;
            GeometryBase geom = speck.Geometry;
            if (null == geom) return;

            Guid id = _doc.Objects.Add(geom);
            if (Guid.Empty == id) return;

            RhinoObject rObj = _doc.Objects.Find(id);

            SyncHost(rObj, speck);
        }

        // TODO : Refactor to TryGetSpeckId pattern?
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
        internal void BakeSpecks(IEnumerable<SpeckInstance> specks)
        {
            var enumer = specks.GetEnumerator();
            while (enumer.MoveNext())
            {
                BakeSpeck(enumer.Current);
            }
            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        #endregion

        #region delete specks

        /// <summary>
        /// Delete a speck from rhino
        /// </summary>
        /// <param name="speck">the speck to delete</param>
        /// This needs to happen in Idle/
        void DeleteSpeck(Guid speckId)
        {
            RemoveSpeck(speckId);

            var _doc = RhinoDoc.ActiveDoc;
            Guid hostId = GetHost(speckId);
            RhinoObject rObj = _doc.Objects.Find(hostId);
            if (rObj is object)
            {
                _doc.Objects.Delete(rObj);
            }
        }
        /// <summary>
        /// Delete multiple specks from rhino
        /// </summary>
        /// <param name="specks">the specks to delete</param>
        void DeleteSpecks(IEnumerable<SpeckInstance> specks)
        {
            if (null == specks) return;

            var enumer = specks.GetEnumerator();
            while (enumer.MoveNext())
            {
                DeleteSpeck(enumer.Current.Id);
            }
        }

        #endregion

        #region Remove Specks
        /// <summary>
        /// Remove a speck for the cache
        /// </summary>
        /// <param name="speck">the speck to remove</param>
        internal void RemoveSpeck(Guid speckId)
        {
            _cache.TryRemove(speckId, out _);
        }

        /// <summary>
        /// Remove multiple specks from the cache
        /// </summary>
        /// <param name="specks">the specks to remove</param>
        internal void RemoveSpecks(IEnumerable<ISpeck> specks)
        {
            if (null == specks) return;

            var enumer = specks.GetEnumerator();
            while (enumer.MoveNext())
            {
                RemoveSpeck(enumer.Current.Id);
            }
        }

        #endregion

        #region events

        // TODO : Move to Idle Queue
        /// <summary>
        /// The speck update events on idle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RhinoApp_Idle(object sender, EventArgs e)
        {
            if (null == ToBake || null == ToRemove) return;

            if (ToBake.Count > 0)
            {
                BakeSpecks(ToBake);
                ToBake.Clear();
            }

            if (ToRemove.Count > 0)
            {
                DeleteSpecks(ToRemove);
                ToRemove.Clear();
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
            DeleteSpeck(speckId);
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

            cacheTable.BakeSpecks(ToBake);
            cacheTable.RemoveSpecks(ToBake);
            SomeoneIsDone = false;

            CrashDoc.ActiveDoc?.Users.Remove(name);
            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        #endregion

    }
}
