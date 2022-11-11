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
    /// Local isntance of a received speck.
    /// </summary>
    public sealed class LocalSpeck : ISpeck
    {
        public DateTime Stamp { get; private set; }

        public Guid Id { get; private set; }

        public string Owner { get; private set; }

        public string? Payload { get; private set; }

        public GeometryBase Geometry { get; private set; }

        private LocalSpeck()
        {
            Stamp = DateTime.UtcNow;
        }

        public static LocalSpeck CreateEmpty()
        {
            return new LocalSpeck()
            {
                Id = Guid.NewGuid()
            };
        }

        public static LocalSpeck Create(Guid id, string? owner, string? payload)
        {
            GeometryBase? geom = CommonObject.FromJSON(payload) as GeometryBase;

            return new LocalSpeck()
            {
                Id = id,
                Owner = owner,
                Geometry = geom,
            };
        }

        public static LocalSpeck Create(string owner, GeometryBase geometry)
        {
            return new LocalSpeck()
            {
                Id = Guid.NewGuid(),
                Owner = owner,
                Geometry = geometry,
                Payload = geometry.ToJSON(null)
            };
        }

        public static LocalSpeck ReCreate(ISpeck speck)
        {
            return Create(speck.Id, speck.Owner, speck.Payload);
        }

    }
}
