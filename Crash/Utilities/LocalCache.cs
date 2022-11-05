using Rhino;
using Rhino.DocObjects;
using SpeckLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Utilities
{
    public sealed class LocalCache
    {
        private ConcurrentDictionary<Guid, Speck> _cache { get; set; }

        private List<Speck> ToBake = new List<Speck>();
        private List<Speck> ToRemove = new List<Speck>();


        public static LocalCache Instance { get; set; }


        public LocalCache()
        {
            RhinoApp.Idle += RhinoApp_Idle;
        }

        #region ConcurrentDictionary Methods
        public async Task UpdateSpeck(Speck speck)
        {
            if (_cache.ContainsKey(speck.Id))
            {
                _cache.TryRemove(speck.Id, out _);
            }

            _cache.TryAdd(speck.Id, speck);
        }
        public IEnumerable<Speck> GetSpecks()
        {
            return _cache.Values;
        }

        #endregion

        #region bake specks

        internal void BakeSpeck(Speck speck)
        {
            var _doc = Rhino.RhinoDoc.ActiveDoc;
            Guid id = _doc.Objects.Add(null); // speck.Geometry);
            RhinoObject rObj = _doc.Objects.Find(id);
            
            // To ensure consistancy
            rObj.Id = speck.Id;
        }

        internal void BakeSpecks(IEnumerable<Speck> specks)
        {
            var enumer = specks.GetEnumerator();
            while(enumer.MoveNext())
            {
                BakeSpeck(enumer.Current);
            }
        }

        #endregion

        #region delete specks

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

        internal void RemoveSpeck(Speck speck)
        {
            _cache.TryRemove(speck.Id, out _);
        }

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

    }
}
