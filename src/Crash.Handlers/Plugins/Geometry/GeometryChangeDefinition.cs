using System.Drawing;

using Crash.Common.Changes;
using Crash.Handlers.Plugins.Geometry.Create;
using Crash.Handlers.Plugins.Geometry.Recieve;

using Rhino.Display;
using Rhino.Geometry;

using Point = Rhino.Geometry.Point;

namespace Crash.Handlers.Plugins.Geometry
{

	/// <summary>Defines the Geometry Change Type to handle default Rhino Geometry</summary>
	public sealed class GeometryChangeDefinition : IChangeDefinition
	{
		/// <inheritdoc/>
		public Type ChangeType => typeof(GeometryChange);

		/// <inheritdoc/>
		public string ChangeName => GeometryChange.ChangeType;

		/// <inheritdoc/>
		public IEnumerable<IChangeCreateAction> CreateActions { get; }

		/// <inheritdoc/>
		public IEnumerable<IChangeRecieveAction> RecieveActions { get; }

		/// <summary>Default Constructor</summary>
		public GeometryChangeDefinition()
		{
			CreateActions = new List<IChangeCreateAction>
			{
				new GeometryCreateAction(),
				new GeometryRemoveAction(),

				// new GeometryTransformAction(),

				new GeometrySelectAction(),
				new GeometryUnSelectAction(),
			};
			RecieveActions = new List<IChangeRecieveAction>
			{
				new GeometryAddRecieveAction(),
				new GeometryTemporaryAddRecieveAction(),
				new GeometryRemoveRecieveAction(),

				// new GeometryTransformRecieveAction(),

				new GeometryLockRecieveAction(),
				new GeometryUnlockRecieveAction(),
			};
		}

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		public BoundingBox GetBoundingBox(IChange change)
		{
			if (change is not GeometryChange geomChange)
				return BoundingBox.Unset;

			return geomChange.Geometry.GetBoundingBox(false);
		}

	}

}
