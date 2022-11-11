using Crash.Utilities;
using Rhino;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;
using SpeckLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using static System.Net.Mime.MediaTypeNames;

namespace Crash.UI
{
    /// <summary>
    /// Interactive pipeline for crash geometry display
    /// </summary>
    public sealed partial class InteractivePipe
    {
        private BoundingBox bbox;

        private bool enabled { get; set; }

        private MeshingParameters meshingParameters = new MeshingParameters(0.5);

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
            if (e.Viewport.ParentView is RhinoPageView) return;

            HashSet<string> owners = new HashSet<string>();
            IEnumerable<Speck> specks = LocalCache.Instance.GetSpecks();
            var enumer = specks.GetEnumerator();
            while(enumer.MoveNext())
            {
                if (e.Display.InterruptDrawing()) return;
                Speck speck = enumer.Current;
                var nameCol = new Utilities.User(speck.Owner).color;
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
        private void UpdateBoundingBox(Speck speck)
        {
            GeometryBase? geom = speck.GetGeom();
            if (geom is object)
            {
                BoundingBox speckBox = geom.GetBoundingBox(false);
                speckBox.Inflate(1.25);
                bbox.Union(speckBox);
            }

        }

    }

}
