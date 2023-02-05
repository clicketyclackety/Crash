using System.Drawing;

using Rhino.Geometry;
using Rhino.Display;
using Rhino;
using Crash.Document;
using Crash.Utilities;
using SpeckLib;
using Eto.Forms;

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
                if (enabled == value) return;

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
            PipeCameraCache = new Dictionary<Color, Line[]>();
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
            if (null == CrashDoc.ActiveDoc?.CacheTable) return;
            bbox = new BoundingBox(-100, -100, -100, 100, 100, 100);

            IEnumerable<SpeckInstance> specks = CrashDoc.ActiveDoc.CacheTable.GetSpecks().OrderBy(s => s.Owner);
            var enumer = specks.GetEnumerator();

            while(enumer.MoveNext())
            {
                SpeckInstance speck = enumer.Current;

                User? user = CrashDoc.ActiveDoc?.Users?.Get(speck.Owner);
                if (user?.Visible != true) continue;

                DrawSpeck(e, speck, user.Color);
                UpdateBoundingBox(speck);
            }

            Dictionary<string, Camera> ActiveCameras = CameraCache.GetActiveCameras();
            foreach(var activeCamera in ActiveCameras)
            {
                User? user = CrashDoc.ActiveDoc?.Users?.Get(activeCamera.Key);
                if (user?.Camera != CameraState.Visible) continue;

                DrawCamera(e, activeCamera.Value, user.Color);
            }
        }

        private Dictionary<Color, Line[]> PipeCameraCache;

        const double width = 1000;
        const double height = 500;
        const int thickness = 4;
        private void DrawCamera(DrawEventArgs e, Camera camera, Color userColor)
        {
            if (e.Display.InterruptDrawing()) return;

            PipeCameraCache.Remove(userColor);
            double ratio = 10;
            double length = 5000 * ratio;

            Vector3d viewAngle = camera.Target - camera.Location;

            Line viewLine = new Line(camera.Location, viewAngle, length);

            Plane cameraFrustrum = new Plane(viewLine.To, viewAngle);
            Interval heightInterval = new Interval(-width * ratio / 2, width * ratio / 2);
            Interval widthInterval = new Interval(-height * ratio / 2, height * ratio / 2);
            Rectangle3d rectangle = new Rectangle3d(cameraFrustrum, widthInterval, heightInterval);

            List<Line> lines = new List<Line>(8);
            lines.AddRange(rectangle.ToPolyline().GetSegments());
            lines.Add(new Line(camera.Location, rectangle.PointAt(0)));
            lines.Add(new Line(camera.Location, rectangle.PointAt(1)));
            lines.Add(new Line(camera.Location, rectangle.PointAt(2)));
            lines.Add(new Line(camera.Location, rectangle.PointAt(3)));

            Interval zInterval = new Interval(0, length);
            BoundingBox cameraBox = new Box(cameraFrustrum, widthInterval, heightInterval, zInterval).BoundingBox;
            bbox.Union(cameraBox);

            foreach(Line line in lines)
            {
                // e.Display.DrawLines(lines, userColor, 3);
                e.Display.DrawPatternedLine(line, userColor, 0x00001111, thickness);
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
