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
            e.IncludeBoundingBox(bbox);
        }

        public void PostDrawObjects(object sender, DrawEventArgs e)
        {
            BoundingBox box = new BoundingBox(0, 0, 0, 100, 200, 300);

            e.Display.DrawBox(box, Color.Red);

            UpdateBoundingBox(e);
        }

        private void UpdateBoundingBox(DrawEventArgs e)
        {
            bbox = new BoundingBox(-1000, -1000, -1000, 1000, 1000, 1000);
        }


    }

}
