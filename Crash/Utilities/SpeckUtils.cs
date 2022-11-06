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
    /// <summary>
    /// Speck utility class
    /// </summary>
    internal static class SpeckUtils
    {
        /// <summary>
        /// Get geometry from speck payload
        /// </summary>
        /// <param name="speck">the speck</param>
        /// <returns></returns>
        public static GeometryBase? GetGeom(this Speck speck)
        {
            try
            {
                var co = GeometryBase.FromJSON(speck.Payload);
                GeometryBase geom = (GeometryBase)co;
                return geom;
            }
            catch(Exception ex)
            {
                ;
            }

            return null;
        }

    }
}
