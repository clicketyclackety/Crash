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
using System.Runtime.InteropServices.WindowsRuntime;
using static System.Net.Mime.MediaTypeNames;

namespace Crash.UI
{
    /// <summary>
    /// Interactive pipeline for crash geometry display
    /// </summary>
    public sealed class InteractivePipe
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
            GeometryBase? geom = speck.GetGeom();
            if (geom == null) return;

            // double transparency = 0.0;

            if (cachedMaterial.Diffuse != color)
            {
                cachedMaterial = new DisplayMaterial(color);
            }


            bool draw = geom switch
            {
                Rhino.Geometry.Point point      => _DrawPoint(e, point),
                
                Curve cv                        => _DrawCurve(e, cv),

                Brep brep                       => _DrawBrep(e, brep),
                Mesh mesh                       => _DrawMesh(e, mesh),
                Extrusion extrusion             => _DrawExtrusion(e, extrusion),
                Surface surface                 => _DrawSurface(e, surface),

                TextEntity text                 => _DrawTextEntity(e, text),
                AnnotationBase anno             => _DrawAnnotation(e, anno),
                TextDot dot                     => _DrawTextDot(e, dot),

                _ => false
            };

        }

        private bool _DrawSurface(DrawEventArgs e, Surface surface)
        {
            /* Of Note -> e.Display.DisplayPipelineAttributes.ShowSurfaceEdges */
            Mesh mesh = Mesh.CreateFromSurface(surface);
            return _DrawMesh(e, mesh);
        }

        private bool _DrawExtrusion(DrawEventArgs e, Extrusion extrusion)
        {
            if (e.Display.DisplayPipelineAttributes.ShadingEnabled)
            {
                Mesh extMesh = extrusion.GetMesh(MeshType.Render);
                e.Display.DrawMeshShaded(extMesh, cachedMaterial);
            }

            e.Display.DrawExtrusionWires(extrusion, cachedMaterial.Diffuse);

            return true;
        }

        private bool _DrawBrep(DrawEventArgs e, Brep brep)
        {
            Mesh mesh = Mesh.CreateFromBrep(brep, meshingParameters).FirstOrDefault();
            return _DrawMesh(e, mesh);
        }

        private bool _DrawMesh(DrawEventArgs e, Mesh mesh)
        {
            var meshAttribs = e.Display.DisplayPipelineAttributes.MeshSpecificAttributes;

            if (meshAttribs.ShowMeshVertices)
                e.Display.DrawMeshVertices(mesh, cachedMaterial.Diffuse);

            if (meshAttribs.ShowMeshWires)
                e.Display.DrawMeshWires(mesh, cachedMaterial.Diffuse, meshAttribs.MeshWireThickness);

            if (e.Display.DisplayPipelineAttributes.ShadingEnabled)
                e.Display.DrawMeshShaded(mesh, cachedMaterial);

            return true;
        }

        private bool _DrawCurve(DrawEventArgs e, Curve curve)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowCurves)
                return false;

            e.Display.DrawCurve(curve, cachedMaterial.Diffuse, e.Display.DefaultCurveThickness);

            return true;
        }

        private bool _DrawPoint(DrawEventArgs e, Rhino.Geometry.Point point)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowPoints)
                return false;

            e.Display.DrawPoint(point.Location,
                                e.Display.DisplayPipelineAttributes.PointStyle,
                                e.Display.DisplayPipelineAttributes.PointRadius,
                                cachedMaterial.Diffuse);

            return true;
        }

        private bool _DrawAnnotation(DrawEventArgs e, AnnotationBase anno)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowAnnotations)
                return false;
             
            e.Display.DrawAnnotation(anno, cachedMaterial.Diffuse);

            return true;
        }

        private bool _DrawTextDot(DrawEventArgs e, TextDot dot)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowAnnotations)
                return false;

            e.Display.DrawDot(dot, Color.White, cachedMaterial.Diffuse, cachedMaterial.Diffuse);

            return true;
        }

        private bool _DrawTextEntity(DrawEventArgs e, TextEntity text)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowText)
                return false;

            e.Display.DrawText(text, cachedMaterial.Diffuse);

            return true;
        }

    }

}
