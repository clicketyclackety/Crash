using System.Collections;

using Crash.Common.Changes;

using Rhino.Geometry;

namespace Crash.Handlers.Tests.Changes
{

	[TestFixture]
	public sealed class GeometryChangeTests
	{

		[TestCaseSource(nameof(ValidGeometry))]
		public void CreateGeometryChange_Successful(GeometryBase geom)
		{
			var change = GeometryChange.CreateNew("Me", geom);
			Assert.That(change, Is.Not.Null);
			Assert.That(change.Geometry, Is.EqualTo(geom));
			Assert.That(change.Payload, Is.Not.Null);

			Assert.That(geom.ToJSON(null), Is.EqualTo(change.Payload));
		}

		[TestCaseSource(nameof(InValidGeometry))]
		public void CreateGeometryChange_Failures(GeometryBase geom)
		{
			// Test
		}

		public static IEnumerable ValidGeometry
		{
			get
			{
				yield return new Point(NRhino.Random.Geometry.NPoint3d.Any());
				yield return NRhino.Random.Geometry.NLineCurve.Any();
				yield return new TextDot("Test", Point3d.Origin);
				yield return new LinearDimension(Plane.WorldXY, new Point2d(-100, -100), new Point2d(100, 100), new Point2d(10, 20));

				var _int = new Interval(-100, 100);
				Box box = new Box(Plane.WorldXY, _int, _int, _int);

				yield return NRhino.Random.Geometry.NMesh.Any();
				yield return Brep.CreateFromBox(box);
				yield return SubD.CreateFromMesh(Mesh.CreateFromBox(box, 10, 10, 10));
			}
		}

		public static IEnumerable InValidGeometry
		{
			get
			{
				yield return null;
				yield return new LineCurve(Point3d.Unset, Point3d.Unset);
				yield return new Point(Point3d.Unset);
				yield return new Brep();
				yield return new Mesh();
			}
		}

	}

}
