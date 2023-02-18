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
    public sealed class SpeckInstance : ISpeck
    {
        ISpeck Speck { get; set; }

        public GeometryBase Geometry { get; private set; }

        public DateTime Stamp => Speck.Stamp;

        public Guid Id => Speck.Id;

        public string Owner => Speck.Owner;

        public bool Temporary => Speck.Temporary;

        public string? LockedBy => Speck.LockedBy;

        public string? Payload => Speck.Payload;


        private SpeckInstance()
        {

        }

        public SpeckInstance(ISpeck speck)
        {
            Speck = speck;
            SerializationOptions options = new SerializationOptions();
            GeometryBase? geometry = CommonObject.FromJSON(speck.Payload) as GeometryBase;
            Geometry = geometry;
        }

        public static SpeckInstance CreateNew(string owner, GeometryBase geometry)
        {
            SerializationOptions options = new SerializationOptions();
            string? payload = geometry?.ToJSON(options);

            Speck speck = new Speck(Guid.NewGuid(), owner, payload);
            SpeckInstance instance = new SpeckInstance(speck) { Geometry = geometry };

            return instance;
        }

    }
}
