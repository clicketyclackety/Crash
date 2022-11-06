using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using SpeckLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Utilities
{
    /// <summary>
    /// Local cache of the collaboration database
    /// </summary>
    public sealed class LocalCache
    {
        public static bool SomeoneIsDone { get; set; }

        private ConcurrentDictionary<Guid, Speck> _cache { get; set; }
        
        //                        <SpeckId, RhinoId>
        private ConcurrentDictionary<Guid, Guid> _SpeckToRhino { get; set; }

        private List<Speck> ToBake = new List<Speck>();
        private List<Speck> ToRemove = new List<Speck>();

        /// <summary>
        /// The instance of the local cache
        /// </summary>
        public static LocalCache Instance { get; set; }

        /// <summary>
        /// Local cache constructor subscribing to RhinoApp_Idle
        /// </summary>
        public LocalCache()
        {
            RhinoApp.Idle += RhinoApp_Idle;
            _cache = new ConcurrentDictionary<Guid, Speck>();
            _SpeckToRhino = new ConcurrentDictionary<Guid, Guid>();
        }

        #region ConcurrentDictionary Methods
        /// <summary>
        /// Method to update a speck
        /// </summary>
        /// <param name="speck">the specks</param>
        /// <returns>returns the update task</returns>
        public async Task UpdateSpeck(Speck speck)
        {
            // Cache
            if (_cache.ContainsKey(speck.Id))
            {
                _cache.TryRemove(speck.Id, out _);
            }

            _cache.TryAdd(speck.Id, speck);
        }

        /// <summary>
        /// Method to get specks
        /// </summary>
        /// <returns>returns a list of the specks</returns>
        public IEnumerable<Speck> GetSpecks()
        {
            return _cache.Values;
        }

        #endregion

        #region bake specks
        /// <summary>
        /// Bake the specks to rhino
        /// </summary>
        /// <param name="speck">the speck to bake</param>
        internal void BakeSpeck(Speck speck)
        {
            var _doc = Rhino.RhinoDoc.ActiveDoc;
            GeometryBase geom = speck.GetGeom();
            Guid id = _doc.Objects.Add(geom);
            RhinoObject rObj = _doc.Objects.Find(id);

            SyncHost(rObj, speck);
        }

        public static void SyncHost(RhinoObject rObj, Speck speck)
        {
            // Data
            string key = "SPECKID";
            if (rObj.UserDictionary.TryGetGuid(key, out _))
            {
                rObj.UserDictionary.Remove(key);
            }

            rObj.UserDictionary.Set(key, speck.Id);

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
        internal void BakeSpecks(IEnumerable<Speck> specks)
        {
            var enumer = specks.GetEnumerator();
            while(enumer.MoveNext())
            {
                BakeSpeck(enumer.Current);
            }
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        #endregion

        #region delete specks
        /// <summary>
        /// Delete a speck from rhino
        /// </summary>
        /// <param name="speck">the speck to delete</param>
        void DeleteSpeck(Speck speck)
        {
            RemoveSpeck(speck);

            var _doc = Rhino.RhinoDoc.ActiveDoc;
            RhinoObject rObj = _doc.Objects.Find(speck.Id);
            if (rObj is object)
            {
                _doc.Objects.Delete(rObj);
            }
        }
        /// <summary>
        /// Delete multiple specks from rhino
        /// </summary>
        /// <param name="specks">the specks to delete</param>
        void DeleteSpecks(IEnumerable<Speck> specks)
        {
            var enumer = specks.GetEnumerator();
            while (enumer.MoveNext())
            {
                DeleteSpeck(enumer.Current);
            }
        }

        #endregion

        #region Remove Specks
        /// <summary>
        /// Remove a speck for the cache
        /// </summary>
        /// <param name="speck">the speck to remove</param>
        internal void RemoveSpeck(Speck speck)
        {
            _cache.TryRemove(speck.Id, out _);
        }

        /// <summary>
        /// Remove multiple specks from the cache
        /// </summary>
        /// <param name="specks">the specks to remove</param>
        internal void RemoveSpecks(IEnumerable<Speck> specks)
        {
            var enumer = specks.GetEnumerator();
            while(enumer.MoveNext())
            {
                RemoveSpeck(enumer.Current);
            }
        }

        #endregion

        #region events
        /// <summary>
        /// The speck update events on idle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RhinoApp_Idle(object sender, EventArgs e)
        {
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
        /// On add event
        /// </summary>
        /// <param name="name">the name </param>
        /// <param name="speck">the speck</param>
        internal static void OnAdd(string name, Speck speck)
        {
            Instance.UpdateSpeck(speck);
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }
        
        /// <summary>
        /// On delete event
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="speckId">speck id</param>
        internal static void OnDelete(string name, Guid speckId)
        {
            Speck speck = new Speck(speckId) { Owner = name };
            Instance.DeleteSpeck(speck);
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        /// <summary>
        /// On update event
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="speckID">the speck id</param>
        /// <param name="speck">the speck</param>
        internal static void OnUpdate(string name, Guid speckID, Speck speck)
        {
            // TODO : ...
        }

        /// <summary>
        /// Collaboration is done event
        /// </summary>
        /// <param name="name">the name of the collaboration</param>
        public static void CollaboratorIsDone(string name)
        {
            SomeoneIsDone = true;
            string sanitisedName = name.ToLower();
            IEnumerable<Speck> ToBake = LocalCache.Instance.GetSpecks().
                                        Where(s => s.Owner.ToLower() == sanitisedName);

            LocalCache.Instance.BakeSpecks(ToBake);
            LocalCache.Instance.RemoveSpecks(ToBake);
            SomeoneIsDone = false;
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        #endregion

    }
}
