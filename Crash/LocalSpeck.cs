using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Runtime;
using SpeckLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash
{

    /// <summary>
    /// Local instance of a received speck.
    /// </summary>
    public sealed class LocalSpeck : Speck
    {

        public GeometryBase Geometry { get; set; }

        private LocalSpeck()
        {

        }

        public static LocalSpeck CreateNew(Guid id, string owner, GeometryBase geometry)
        {
            SerializationOptions options = new SerializationOptions();

            var speck = new LocalSpeck()
            {
                Id = id,
                Geometry = geometry,
                Payload = geometry?.ToJSON(options),
                Owner = owner,
                LockedBy = owner,
                Stamp = DateTime.UtcNow,
                Temporary = true
            };

            return speck;
        }

        public static LocalSpeck ReCreate(Speck speck)
        {
            GeometryBase geometry = CommonObject.FromJSON(speck.Payload) as GeometryBase;
            var localSpeck = new LocalSpeck()
            {
                Id = speck.Id,
                Geometry = geometry,
                Owner = speck.Owner,
                Payload = speck.Payload,
                Temporary = speck.Temporary,
                Stamp = speck.Stamp,
                LockedBy = speck.LockedBy
            };

            return localSpeck;
        }


    }
}
