using System.Drawing;
using System.Linq;

using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Display;
using Rhino;

using SpeckLib;

using Crash.Utilities;

namespace Crash.UI
{

    public sealed partial class InteractivePipe
    {

        /// Re-using materials is much faster
        DisplayMaterial cachedMaterial = new DisplayMaterial(Color.Blue);
        Color cachedColour => cachedMaterial.Diffuse;

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
            if (cachedColour != color)
            {
                cachedMaterial = new DisplayMaterial(color);
            }

            _ = geom switch
            {
                Rhino.Geometry.Point point => _DrawPoint(e, point),

                Curve cv => _DrawCurve(e, cv),

                Mesh mesh => _DrawMesh(e, mesh),
                Extrusion extrusion => _DrawExtrusion(e, extrusion),
                Surface surface => _DrawSurface(e, surface),
                Brep brep => _DrawBrep(e, brep),

                TextEntity text => _DrawTextEntity(e, text),
                TextDot dot => _DrawTextDot(e, dot),

                LinearDimension linearDimension => _DrawLinearDimension(e, linearDimension),
                RadialDimension radialDimension => _DrawRadialDimension(e, radialDimension),
                OrdinateDimension ordinateDimension => _DrawOrdinateDimension(e, ordinateDimension),
                AngularDimension angularDimension => _DrawAngularDimension(e, angularDimension),

                Centermark centermark => _DrawCentermark(e, centermark),
                Leader leader => _DrawLeader(e, leader),

                AnnotationBase anno => _DrawAnnotation(e, anno),

                _ => false
            };

        }

        private bool _DrawSurface(DrawEventArgs e, Surface surface)
        {
            if (surface.IsSphere(0.1))
            {
                if (!surface.TryGetSphere(out Sphere sphere)) return false;
                e.Display.DrawSphere(sphere, cachedColour);

                return true;
            }

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

            e.Display.DrawExtrusionWires(extrusion, cachedColour);

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
                e.Display.DrawMeshVertices(mesh, cachedColour);

            if (meshAttribs.ShowMeshWires)
                e.Display.DrawMeshWires(mesh, cachedColour, meshAttribs.MeshWireThickness);

            if (e.Display.DisplayPipelineAttributes.ShadingEnabled)
                e.Display.DrawMeshShaded(mesh, cachedMaterial);

            return true;
        }

        private bool _DrawCurve(DrawEventArgs e, Curve curve)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowCurves)
                return false;

            e.Display.DrawCurve(curve, cachedColour, e.Display.DefaultCurveThickness);

            return true;
        }

        private bool _DrawPoint(DrawEventArgs e, Rhino.Geometry.Point point)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowPoints)
                return false;

            e.Display.DrawPoint(point.Location,
                                e.Display.DisplayPipelineAttributes.PointStyle,
                                e.Display.DisplayPipelineAttributes.PointRadius,
                                cachedColour);

            return true;
        }

        private bool _DrawAnnotation(DrawEventArgs e, AnnotationBase anno)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowAnnotations)
                return false;

            e.Display.DrawAnnotation(anno, cachedColour);

            return true;
        }

        private bool _DrawTextDot(DrawEventArgs e, TextDot dot)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowAnnotations)
                return false;

            e.Display.DrawDot(dot, Color.White, cachedColour, cachedColour);

            return true;
        }

        private bool _DrawTextEntity(DrawEventArgs e, TextEntity text)
        {
            if (!e.Display.DisplayPipelineAttributes.ShowText)
                return false;

            var style = Rhino.RhinoDoc.ActiveDoc.DimStyles.Current;
            double scale = style.TextHeight * style.DimensionScale;
            e.Display.DrawText(text, cachedColour, scale);

            return true;
        }


        private TextEntity _GetTextFromLinearDimension(LinearDimension linearDimension)
        {
            UnitSystem unitSys = RhinoDoc.ActiveDoc.ModelUnitSystem;
            DimensionStyle dimStyle = RhinoDoc.ActiveDoc.DimStyles.Current;

            linearDimension.GetTextRectangle(out Point3d[] corners);
            double rectWidth = corners[0].DistanceTo(corners[2]);

            linearDimension.Get3dPoints(out Point3d e1End, out Point3d e2End,
                                        out Point3d a1End, out Point3d a2End,
                                        out _, out Point3d textPoint);

            Point3d extensionMid = new Line(e1End, e2End).PointAt(0.5);
            Plane plane = new Plane(textPoint, a1End, extensionMid);
            plane.Translate(-plane.YAxis * 25);

            string text = $"{linearDimension.GetDistanceDisplayText(unitSys, dimStyle)}";
            TextEntity textEntity = TextEntity.Create(text, plane, dimStyle, false, rectWidth, 0);
            double distance = (textEntity.TextModelWidth * textEntity.DimensionScale) / 2;
            Vector3d direction = a2End - a1End;
            direction.Unitize();
            textEntity.Translate(direction * distance);

            return textEntity;
        }

        private TextEntity _GetTextFromAngularDimension(AngularDimension angularDimension)
        {
            DimensionStyle dimStyle = RhinoDoc.ActiveDoc.DimStyles.Current;

            angularDimension.Get3dPoints(out Point3d centPoint, out _, out _,
                                         out Point3d arrowPoint1, out Point3d arrowPoint2,
                                         out Point3d DimlP, out Point3d textPoint);
            Circle circ = new Circle(arrowPoint2, DimlP, arrowPoint1);
            circ.ClosestParameter(textPoint, out double textParam);
            Vector3d xVec = circ.TangentAt(textParam);

            angularDimension.GetTextRectangle(out Point3d[] corners);
            double rectWidth = corners[0].DistanceTo(corners[2]);

            Vector3d yVec = Vector3d.CrossProduct(xVec, angularDimension.Plane.ZAxis);
            Plane plane = new Plane(textPoint, xVec, yVec);

            string text = $"{angularDimension.GetAngleDisplayText(dimStyle)}";
            TextEntity textEntity = TextEntity.Create(text, plane, dimStyle, false, rectWidth, 0);

            return textEntity;
        }

        private TextEntity _GetTextFromRadialDimension(RadialDimension radialDimension)
        {
            UnitSystem unitSys = RhinoDoc.ActiveDoc.ModelUnitSystem;
            DimensionStyle dimStyle = RhinoDoc.ActiveDoc.DimStyles.Current;

            radialDimension.GetTextRectangle(out Point3d[] corners);
            double rectWidth = corners[0].DistanceTo(corners[2]);

            radialDimension.Get3dPoints(out Point3d centPoint, out Point3d radiusPoint,
                                        out Point3d dimLinePoint, out Point3d kneePoint);

            Plane plane = new Plane(dimLinePoint, Vector3d.XAxis, Vector3d.YAxis);

            string text = $"{radialDimension.GetDistanceDisplayText(unitSys, dimStyle)}";
            TextEntity textEntity = TextEntity.Create(text, plane, dimStyle, false, rectWidth, 0);

            return textEntity;
        }

        private TextEntity _GetTextFromOrdinateDimension(OrdinateDimension ordinateDimension)
        {
            // FIXME : Fix alignment issues

            UnitSystem unitSys = RhinoDoc.ActiveDoc.ModelUnitSystem;
            DimensionStyle dimStyle = RhinoDoc.ActiveDoc.DimStyles.Current;

            ordinateDimension.GetTextRectangle(out Point3d[] corners);
            double rectWidth = corners[0].DistanceTo(corners[2]);

            ordinateDimension.Get3dPoints(out Point3d centPoint, out Point3d defPoint,
                            out Point3d dimPoint, out Point3d kinkPoint1, out Point3d kinkPoint2);

            Plane plane = new Plane(dimPoint, ordinateDimension.Plane.XAxis, ordinateDimension.Plane.YAxis);

            string text = $"{ordinateDimension.GetDistanceDisplayText(unitSys, dimStyle)}";
            TextEntity textEntity = TextEntity.Create(text, plane, dimStyle, false, rectWidth, 0);

            return textEntity;
        }

        private TextEntity _GetTextFromLeader(Leader leader)
        {
            // FIXME : Fix alignment issues

            UnitSystem unitSys = RhinoDoc.ActiveDoc.ModelUnitSystem;
            DimensionStyle dimStyle = RhinoDoc.ActiveDoc.DimStyles.Current;

            Plane plane = new Plane(leader.Points3D.Last(), leader.Plane.XAxis, leader.Plane.YAxis);

            double rectWidth = leader.TextModelWidth;

            /*  leader.PlainText gives something along the lines of %<Area(“8974f391-92da-413b-bd36-a2bd263ee91e”)>% as a value
             *  RhinoApp.ParseTextField(...) is the only way to parse this. But that requires the rhino object as an input
             *  Which this method cannot possibly access.
            */
            string text = "Not Supported";
            TextEntity textEntity = TextEntity.Create(text, plane, dimStyle, false, rectWidth, 0);

            return textEntity;
        }

        private bool _DrawLinearDimension(DrawEventArgs e, LinearDimension linearDimension)
        {
            TextEntity textEntity = _GetTextFromLinearDimension(linearDimension);

            linearDimension.Get3dPoints(out _, out _, out Point3d arrow1End, out Point3d arrow2End, out _, out _);
            Line arrowLine = new Line(arrow1End, arrow2End);

            if (linearDimension.ArrowFit != DimensionStyle.ArrowFit.ArrowsOutside)
            {
                _DrawArrow(e, arrow1End, -arrowLine.UnitTangent);
                _DrawArrow(e, arrow2End, arrowLine.UnitTangent);
            }
            else
            {
                // TO DO : Support other Arrow Styles
            }

            _DrawAnnotation(e, linearDimension);
            _DrawAnnotation(e, textEntity);

            return true;
        }

        private void _DrawArrow(DrawEventArgs e, Point3d arrowEnd, Vector3d direction)
        {
            e.Display.DrawArrowHead(arrowEnd, direction, cachedColour, 0, _GetArrowSize(null));
        }

        private double _GetArrowSize(Dimension dimension)
        {
            double size = Rhino.RhinoDoc.ActiveDoc.DimStyles.Current.DimensionScale;
            double arrowSize = Rhino.RhinoDoc.ActiveDoc.DimStyles.Current.ArrowLength;

            return size * arrowSize;
        }

        private bool _DrawRadialDimension(DrawEventArgs e, RadialDimension radialDimension)
        {
            TextEntity textEntity = _GetTextFromRadialDimension(radialDimension);
            // FIXME : This translate needs fixing, it always moves left
            textEntity.Translate(-radialDimension.Plane.XAxis * textEntity.TextModelWidth * textEntity.DimensionScale);

            radialDimension.Get3dPoints(out _, out Point3d radiusPoint, out Point3d dimLinePoint, out _);
            Vector3d arrowVec = new Line(dimLinePoint, radiusPoint).UnitTangent;
            _DrawArrow(e, radiusPoint, arrowVec);

            _DrawRadialCross(e, radialDimension);
            _DrawAnnotation(e, radialDimension);
            _DrawAnnotation(e, textEntity);

            return true;
        }

        private bool _DrawOrdinateDimension(DrawEventArgs e, OrdinateDimension ordinateDimension)
        {
            TextEntity textEntity = _GetTextFromOrdinateDimension(ordinateDimension);

            _DrawAnnotation(e, ordinateDimension);
            _DrawAnnotation(e, textEntity);

            return true;
        }

        private bool _DrawAngularDimension(DrawEventArgs e, AngularDimension angularDimension)
        {
            TextEntity textEntity = _GetTextFromAngularDimension(angularDimension);

            angularDimension.Get3dPoints(out _, out _, out _,
                                         out Point3d arrowPoint1, out Point3d arrowPoint2,
                                         out Point3d DimlP, out Point3d textPoint);
            Circle circ = new Circle(arrowPoint2, DimlP, arrowPoint1);

            circ.ClosestParameter(arrowPoint1, out double arrParam1);
            circ.ClosestParameter(arrowPoint2, out double arrParam2);

            Vector3d arrowDirection1 = circ.TangentAt(arrParam1);
            Vector3d arrowDirection2 = circ.TangentAt(arrParam2);

            if (angularDimension.ArrowFit != DimensionStyle.ArrowFit.ArrowsOutside)
            {
                _DrawArrow(e, arrowPoint1, arrowDirection1);
                _DrawArrow(e, arrowPoint2, -arrowDirection2);
            }
            else
            {
                // TO DO : Support other Arrow Styles
            }

            _DrawAnnotation(e, angularDimension);
            _DrawAnnotation(e, textEntity);

            return true;
        }

        private bool _DrawCentermark(DrawEventArgs e, Centermark centermark)
        {
            return false;
        }

        private void _DrawRadialCross(DrawEventArgs e, RadialDimension radialDimension)
        {
            // TODO : Scale this properly.
            double scale = Rhino.RhinoDoc.ActiveDoc.DimStyles.Current.DimensionScale;
            double length = 750; //  radialDimension.CentermarkSize * scale;
            radialDimension.Get3dPoints(out Point3d centPoint, out _, out _, out _);
            Plane plane = radialDimension.Plane;

            Point3d upPoint = centPoint;
            Point3d downPoint = centPoint;
            Point3d leftPoint = centPoint;
            Point3d rightPoint = centPoint;

            Vector3d upDown = plane.YAxis * (length / 2);
            Vector3d leftRight = plane.XAxis * (length / 2);

            upPoint.Transform(Transform.Translation(-upDown));
            downPoint.Transform(Transform.Translation(upDown));
            leftPoint.Transform(Transform.Translation(-leftRight));
            rightPoint.Transform(Transform.Translation(leftRight));

            Line upDownLine = new Line(upPoint, downPoint);
            Line rightLeftLine = new Line(leftPoint, rightPoint);

            e.Display.DrawLine(upDownLine, cachedColour);
            e.Display.DrawLine(rightLeftLine, cachedColour);
        }

        private bool _DrawLeader(DrawEventArgs e, Leader leader)
        {
            TextEntity textEntity = _GetTextFromLeader(leader);

            _DrawAnnotation(e, leader);
            _DrawAnnotation(e, textEntity);

            return true;
        }

    }

}
