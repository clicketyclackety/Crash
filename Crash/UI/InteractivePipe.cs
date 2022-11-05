using Rhino.Display;
using Rhino.Geometry;
using SpeckLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.UI
{
    public sealed class InteractivePipe
    {
        private BoundingBox bbox;
        private bool enabled { get; set; }

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;

                    if (enabled)
                    {
                        DisplayPipeline.CalculateBoundingBox += CalculateBoundingBox;
                        DisplayPipeline.PostDrawObjects += PostDrawObjects;
                    }
                    else
                    {
                        DisplayPipeline.CalculateBoundingBox -= CalculateBoundingBox;
                        DisplayPipeline.PostDrawObjects -= PostDrawObjects;
                    }
                }
            }
        }

        internal ConcurrentDictionary<Guid, Speck> Drawables { get; set; }

        public InteractivePipe Instance;

        public InteractivePipe()
        {
            Drawables = new ConcurrentDictionary<Guid, Speck>();
            Instance = this;
        }


        public void CalculateBoundingBox(object sender, CalculateBoundingBoxEventArgs e)
        {
            // TODO : Iterate through specks, add boundingbox of geometry to bbox;

            // Dummy box
            e.IncludeBoundingBox(bbox);
        }

        public void PostDrawObjects(object sender, DrawEventArgs e)
        {
            BoundingBox box = new BoundingBox(0, 0, 0, 100, 200, 300);

            e.Display.DrawBox(box, Color.Red);

            IEnumerable<Speck> specks = Drawables.Values.ToList().OrderBy(s => s.Owner);
            var enumer = specks.GetEnumerator();
            while(enumer.MoveNext())
            {
                Speck speck = enumer.Current;
                Color nameCol = Utilities.User.UserColor(speck.Owner);
                DrawSpeck(e, speck, nameCol);
            }

            UpdateBoundingBox(e);
        }

        private void UpdateBoundingBox(DrawEventArgs e)
        {
            bbox = new BoundingBox(-1000, -1000, -1000, 1000, 1000, 1000);
        }

        DisplayMaterial cachedMaterial = new DisplayMaterial(Color.Blue);
        private void DrawSpeck(DrawEventArgs e, Speck speck, Color color)
        {
            GeometryBase geom = null; // speck.Geometry;
            if (cachedMaterial.Diffuse != color)
            {
                cachedMaterial = new DisplayMaterial(color);
            }

            if (geom is Curve cv)
            {
                e.Display.DrawCurve(cv, color);
            }
            else if (geom is Brep brep)
            {
                e.Display.DrawBrepShaded(brep, cachedMaterial);
            }
            else if (geom is Mesh mesh)
            {
                e.Display.DrawMeshShaded(mesh, cachedMaterial);
            }
        }

        private void DrawNothing() { }

        private void DrawCurve(Curve curve)
        {

        }


    }

}
