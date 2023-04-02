﻿using System.Drawing;

using Crash.Common.Changes;
using Crash.Handlers.Plugins.Geometry.Create;
using Crash.Handlers.Plugins.Geometry.Recieve;

using Rhino.Display;
using Rhino.Geometry;

using Point = Rhino.Geometry.Point;

namespace Crash.Handlers.Plugins.Geometry
{

	public sealed class GeometryChangeDefinition : IChangeDefinition
	{
		public Type ChangeType => typeof(GeometryChange);

		public string ChangeName => $"{nameof(Crash)}.{nameof(GeometryChange)}";

		public IEnumerable<IChangeCreateAction> CreateActions { get; }

		public IEnumerable<IChangeRecieveAction> RecieveActions { get; }


		public GeometryChangeDefinition()
		{
			var createActions = new List<IChangeCreateAction>
			{
				new GeometryCreateAction(),
				new GeometryRemoveAction(),

				// new GeometryTransformAction(),

				new GeometrySelectAction(),
				new GeometryUnSelectAction(),
			};
			var receveActions = new List<IChangeRecieveAction>
			{
				new GeometryAddRecieveAction(),
				new GeometryTemporaryAddRecieveAction(),
				new GeometryRemoveRecieveAction(),

				// new GeometryTransformRecieveAction(),

				new GeometryLockRecieveAction(),
				new GeometryUnlockRecieveAction(),
			};
		}


		public void Draw(DrawEventArgs drawArgs, DisplayMaterial material, IChange change)
		{
			if (change is not GeometryChange geomChange) return;

			var geom = geomChange.Geometry;
			if (geom is Curve cv)
			{
				drawArgs.Display.DrawCurve(cv, material.Diffuse, 2);
			}
			else if (geom is Brep brep)
			{
				drawArgs.Display.DrawBrepShaded(brep, material);
			}
			else if (geom is Mesh mesh)
			{
				drawArgs.Display.DrawMeshShaded(mesh, material);
			}
			else if (geom is Extrusion ext)
			{
				drawArgs.Display.DrawExtrusionWires(ext, material.Diffuse);
			}
			else if (geom is TextEntity te)
			{
				drawArgs.Display.DrawText(te, material.Diffuse);
			}
			else if (geom is TextDot td)
			{
				drawArgs.Display.DrawDot(td, Color.White, material.Diffuse, material.Diffuse);
			}
			else if (geom is Surface srf)
			{
				drawArgs.Display.DrawSurface(srf, material.Diffuse, 1);
			}
			else if (geom is Point pnt)
			{
				drawArgs.Display.DrawPoint(pnt.Location, material.Diffuse);
			}
			else if (geom is AnnotationBase ab)
			{
				drawArgs.Display.DrawAnnotation(ab, material.Diffuse);
			}
		}

		public BoundingBox GetBoundingBox(IChange change)
		{
			if (change is not GeometryChange geomChange)
				return BoundingBox.Unset;

			return geomChange.Geometry.GetBoundingBox(false);
		}

	}

}
