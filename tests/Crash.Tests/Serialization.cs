using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Geometry;
using Crash.Common.Document;
using Crash.Common.Changes;

namespace Crash.Tests
{

    [TestClass]
    public class Serialization
    {

        [TestMethod]
        public void TestCamera()
        {
            User user = new User("MrMan");
            Camera camera = new Camera(new Point3d(100, 200, 300), Point3d.Origin);

            string json = camera.ToJSON();
            Assert.IsFalse(string.IsNullOrEmpty(json));

            Camera cameraBack = Camera.FromJSON(json);
            Assert.IsNotNull(cameraBack);
        }

    }

}
