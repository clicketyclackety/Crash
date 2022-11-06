using Rhino.Geometry;
using SpeckLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Utilities
{
    internal static class SpeckUtils
    {

        public static GeometryBase? GetGeom(this Speck speck)
        {
            GeometryBase geom = (GeometryBase)GeometryBase.FromJSON(speck.Payload);
            return geom;
        }

    }
}
