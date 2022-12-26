using Crash.Utilities;
using Rhino;
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
    /// <summary>
    /// Interactive pipeline for crash geometry display
    /// </summary>
    public sealed class InteractivePipe
    {
        private BoundingBox bbox;

        private bool enabled { get; set; }

        /// <summary>
        /// Pipeline enabled, disabling hides it
        /// </summary>
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

        /// <summary>
        /// The interactive pipeline instance
        /// </summary>
        public static InteractivePipe Instance;

        /// <summary>
        /// Empty constructor
        /// </summary>
        public InteractivePipe()
        {
            Instance = this;
            bbox = new BoundingBox(-100, -100, -100, 100, 100, 100);
        }

        /// <summary>
        /// Method to calculate the bounding box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CalculateBoundingBox(object sender, CalculateBoundingBoxEventArgs e)
        {
            e.IncludeBoundingBox(bbox);
        }

        /// <summary>
        /// Post draw object events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PostDrawObjects(object sender, DrawEventArgs e)
        {
            HashSet<string> owners = new HashSet<string>();
            if (null == LocalCache.Instance) return;
            IEnumerable<SpeckInstance> specks = LocalCache.Instance.GetSpecks().OrderBy(s => s.Owner);
            var enumer = specks.GetEnumerator();
            while(enumer.MoveNext())
            {
                SpeckInstance speck = enumer.Current;
                var nameCol = new User(speck.Owner).color;
                DrawSpeck(e, speck, nameCol);
                owners.Add(speck.Owner);
                UpdateBoundingBox(speck);
            }

            var userEnumer = owners.GetEnumerator();
            int counter = 0;
            while (userEnumer.MoveNext())
            {
                string user = userEnumer.Current;
                var nameCol = new User(user).color;
                DrawUser(e, user, nameCol,counter);
                counter++;
            }
        }

        private void DrawUser(DrawEventArgs e, string user, Color color, int counter)
        {
            try
            {
                Rectangle rect = RhinoDoc.ActiveDoc.Views.ActiveView.Bounds;
                int xCoord = rect.X+50;
                int yCoord = rect.Y+50;
                yCoord += counter * 30;
                Point2d point = new Point2d(xCoord, yCoord);
                e.Display.Draw2dText(user, color, point, false, 20);
            }
            
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Updates the BoundingBox of the Pipeline
        /// </summary>
        /// <param name="speck"></param>
        private void UpdateBoundingBox(SpeckInstance speck)
        {
            GeometryBase? geom = speck.Geometry;
            if (geom is object)
            {
                BoundingBox speckBox = geom.GetBoundingBox(false);
                speckBox.Inflate(1.25);
                bbox.Union(speckBox);
            }

        }

        /// Re-using materials is much faster
        DisplayMaterial cachedMaterial = new DisplayMaterial(Color.Blue);
        /// <summary>
        /// Draws a Speck in the pipeline.
        /// </summary>
        /// <param name="e">The EventArgs from the DisplayConduit</param>
        /// <param name="speck">The Speck</param>
        /// <param name="color">The colour for the speck, based on the user.</param>
        private void DrawSpeck(DrawEventArgs e, SpeckInstance speck, Color color)
        {
            GeometryBase? geom = speck.Geometry;
            if (geom == null) return;

            if (cachedMaterial.Diffuse != color)
            {
                cachedMaterial = new DisplayMaterial(color);
            }

            try
            {
                if (geom is Curve cv)
                {
                    e.Display.DrawCurve(cv, color, 2);
                }
                else if (geom is Brep brep)
                {
                    e.Display.DrawBrepShaded(brep, cachedMaterial);
                }
                else if (geom is Mesh mesh)
                {
                    e.Display.DrawMeshShaded(mesh, cachedMaterial);
                }
                else if (geom is Extrusion ext)
                {
                    e.Display.DrawExtrusionWires(ext, cachedMaterial.Diffuse);
                }
                else if (geom is TextEntity te)
                {
                    e.Display.DrawText(te, cachedMaterial.Diffuse);
                }
                else if (geom is TextDot td)
                {
                    e.Display.DrawDot(td, Color.White, cachedMaterial.Diffuse, cachedMaterial.Diffuse);
                }
                else if (geom is Surface srf)
                {
                    e.Display.DrawSurface(srf, cachedMaterial.Diffuse, 1);
                }
                else if (geom is Rhino.Geometry.Point pnt)
                {
                    e.Display.DrawPoint(pnt.Location, cachedMaterial.Diffuse);
                }
                else if (geom is AnnotationBase ab)
                {
                    e.Display.DrawAnnotation(ab, cachedMaterial.Diffuse);
                }
                else
                {
                    ;
                }
            }
            catch(Exception)
            {
                
            }
        }

    }

}
