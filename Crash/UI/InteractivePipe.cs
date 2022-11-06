using Crash.Events;
using Crash.Utilities;
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
        /// The drawable items
        /// </summary>
        internal ConcurrentDictionary<Guid, Speck> Drawables { get; set; }

        /// <summary>
        /// The interactive pipeline instance
        /// </summary>
        public InteractivePipe Instance;

        /// <summary>
        /// Empty constructor
        /// </summary>
        public InteractivePipe()
        {
            Drawables = new ConcurrentDictionary<Guid, Speck>();
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
            IEnumerable<Speck> specks = LocalCache.Instance.GetSpecks();
            var enumer = specks.GetEnumerator();
            while(enumer.MoveNext())
            {
                Speck speck = enumer.Current;
                var nameCol = new Utilities.User(speck.Owner).color;
                DrawSpeck(e, speck, nameCol);

                UpdateBoundingBox(speck);
            }

            if (MouseEvent.selectedUser == string.Empty || MouseEvent.tagLocation == Point3d.Unset)
                return;
            else
            {
                var nameCol = new Utilities.User(MouseEvent.selectedUser).color;
                
                e.Display.Draw2dText(MouseEvent.selectedUser, nameCol, new Point2d(MouseEvent.tagLocation.X,MouseEvent.tagLocation.Y), true, 100);
            }
        }

        /// <summary>
        /// Updates the BoundingBox of the Pipeline
        /// </summary>
        /// <param name="speck"></param>
        private void UpdateBoundingBox(Speck speck)
        {
            BoundingBox speckBox = speck.GetGeom().GetBoundingBox(false);
            speckBox.Inflate(1.25);
            bbox.Union(speckBox);
        }

        /// Re-using materials is much faster
        DisplayMaterial cachedMaterial = new DisplayMaterial(Color.Blue);
        /// <summary>
        /// Draws a Speck in the pipeline.
        /// </summary>
        /// <param name="e">The EventArgs from the DisplayConduit</param>
        /// <param name="speck">The Speck</param>
        /// <param name="color">The colour for the speck, based on the user.</param>
        private void DrawSpeck(DrawEventArgs e, Speck speck, Color color)
        {
            GeometryBase geom = speck.GetGeom();
            if (cachedMaterial.Diffuse != color)
            {
                cachedMaterial = new DisplayMaterial(color);
            }

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
            else
            {
                ;
            }
        }

    }

}
